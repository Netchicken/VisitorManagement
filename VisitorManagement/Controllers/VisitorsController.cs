using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VisitorManagement.Business;
using VisitorManagement.Data;
using VisitorManagement.Models;

namespace VisitorManagement.Controllers
{
    public class VisitorsController : Controller
    {
        private readonly VisitorDbContext _context;
        private readonly IDataBaseCalls _dataBaseCalls;
        public VisitorsController(VisitorDbContext context, IDataBaseCalls dataBaseCalls)
        {
            _context = context;
            _dataBaseCalls = dataBaseCalls;
        }

        // GET: Visitors
        public async Task<IActionResult> Index()
        {

            //shows all visitors that havn't logged out
            List<Visitor> VisitorLogOut = new List<Visitor>();

            VisitorLogOut.AddRange(_context.Visitor.OrderBy(v => v.FirstName).Where(v => v.DateOut == default(DateTime)).ToList());

            ViewData["VisitorLogOut"] = VisitorLogOut;
            return View(VisitorLogOut);

            //   return View(_context.Visitor.OrderBy(v => v.FirstName).Where(v => v.DateOut == default(DateTime)).ToList());
        }

        // GET: Visitors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // GET: Visitors/Create
        public IActionResult Create()
        {
            //  ViewBag.Staff = new SelectList(_context.StaffNames, "Id", "Name");
            //ViewData["Staff"] = _context.StaffNames.Select(n => new SelectListItem
            //{
            //    Value = n.Id.ToString(),
            //    Text = n.Name + " " + n.Department
            //}).ToList();
            //

            ViewData["ReturningVisitors"] = _context.Visitor.Distinct()
                .OrderBy(n => n.FirstName)
                .Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.FirstName + " " + n.LastName
                }).ToList();


            return View();
        }

        // POST: Visitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Business,DateIn,DateOut,StaffName")] Visitor visitor)
        {
            if (ModelState.IsValid)
            {
                //increment staff count by 1
                _dataBaseCalls.IncrementStaffCount(visitor.StaffName.Id);


                //add the date in to the visitor
                visitor.DateIn = DateTime.Now;
                _context.Add(visitor);
                await _context.SaveChangesAsync();

                return Redirect("~/Home/Index");
                //  return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        // GET: Visitors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitor.FindAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }


            return View(visitor);
        }
        //log out
        // POST: Visitors/LogOut/5
        public async Task<IActionResult> LogOut(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Visitor visitor = await _context.Visitor.FindAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                visitor.DateOut = DateTime.Now;
                _context.Update(visitor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        // POST: Visitors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        //todo we have to get the post again
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Business,DateIn,DateOut")] Visitor visitor)
        {
            if (id != visitor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    visitor.DateOut = DateTime.Now;
                    _context.Update(visitor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitorExists(visitor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(visitor);
        }

        // GET: Visitors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // POST: Visitors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitor = await _context.Visitor.FindAsync(id);
            _context.Visitor.Remove(visitor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitorExists(int id)
        {
            return _context.Visitor.Any(e => e.Id == id);
        }

        //[HttpPost]
        //public IActionResult GetParticalView(string selectvalue)
        //{
        //    //... it could get selectvalue value successfully
        //    //then add _GetParticalView.cshtml PartialView view page
        //    //then pass query data to  _GetParticalView.cshtml
        //    return PartialView("GetParticalView");

        //}
    }
}
