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
            //code is in Home Controller / Index  - not below

            //  ViewBag.Staff = new SelectList(_context.StaffNames, "Id", "Name");
            //ViewData["Staff"] = _context.StaffNames.Select(n => new SelectListItem
            //{
            //    Value = n.Id.ToString(),
            //    Text = n.Name + " " + n.Department
            //}).ToList();
            //

            //ViewData["ReturningVisitors"] = _context.Visitor.Distinct()
            //    .OrderBy(n => n.FirstName)
            //    .Select(n => new SelectListItem
            //    {
            //        Value = n.Id.ToString(),
            //        Text = n.FirstName + " " + n.LastName + " " +n.StaffName.Name
            //    }).ToList();

            //ViewData["ReturningVisitors"] = new SelectList(_context.Visitor.Distinct().OrderBy(n => n.FirstName), "Id",
            //    "FirstName" + " " + "LastName" + " " + "StaffName.Name").ToList();


            return View();
        }

        // POST: Visitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReturningVisitor,FirstName,LastName,Business,DateIn,DateOut,StaffName")] CreateVisitorVM createVisitorVM)
        {

            if (createVisitorVM.ReturningVisitor != null)
            {

                //get the entry at that ID
                int id = Convert.ToInt32(createVisitorVM.ReturningVisitor);
                //get the details of the returning visitor without the ID
                Visitor newvisitor = (Visitor)_context.Visitor
                    .Where(v => v.Id == id)
                    .Select(v => new Visitor { FirstName = v.FirstName, LastName = v.LastName, StaffName = v.StaffName, Business = v.Business })
                    .SingleOrDefault();


                // Get the staff ID from the DB via the name
                var StaffID = _context.StaffNames.Where(s => s.Name == newvisitor.StaffName.ToString()).Select(s => s.Id).SingleOrDefaultAsync();

                //increment the staff count by 1
                int staffid = Convert.ToInt16(StaffID);
                _dataBaseCalls.IncrementStaffCount(staffid);

                //reset the id to 0 to make it a new entry
                // newvisitor.Id = 0;
                //add the date in to the visitor
                newvisitor.DateIn = DateTime.Now;
                _context.Add(newvisitor);
                await _context.SaveChangesAsync();

                return Redirect("~/Home/Index");
            }

            if (ModelState.IsValid)
            {
                //increment staff count by 1
                _dataBaseCalls.IncrementStaffCount(createVisitorVM.StaffName.Id);

                //add the date in to the visitor
                createVisitorVM.DateIn = DateTime.Now;

                Visitor visitor = new Visitor();

                visitor.Business = createVisitorVM.Business;
                visitor.DateIn = createVisitorVM.DateIn;
                visitor.DateOut = createVisitorVM.DateOut;
                visitor.FirstName = createVisitorVM.FirstName;
                visitor.LastName = createVisitorVM.LastName;
                visitor.StaffName = createVisitorVM.StaffName;

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
