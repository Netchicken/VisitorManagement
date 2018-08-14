using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VisitorManagement.Business;
using VisitorManagement.Data;
using VisitorManagement.Models;

namespace VisitorManagement.Controllers
{
    public class HomeController : Controller
    {

        public VisitorDbContext _context { get; }
        private ITextFileOperations _textFileOperations;
        private readonly IDataBaseCalls _dataBaseCalls;

        public HomeController(VisitorDbContext context, ITextFileOperations textFileOperations, IDataBaseCalls dataBaseCalls)
        {
            _textFileOperations = textFileOperations;
            _dataBaseCalls = dataBaseCalls;
            _context = context;
        }


        public IActionResult Index()
        {
            //https://www.learnrazorpages.com/razor-pages/tag-helpers/select-tag-helper

            List<SelectListItem> Top5Staff = new List<SelectListItem>();


            ViewData["Staff"] = ListOfStaff(Top5Staff);


            ViewData["Conditions"] = _textFileOperations.LoadConditionsForAcceptanceText();


            //var ReturningVisitors = _context.Visitor.OrderBy(n => n.FirstName)
            //    .Select(v =>
            //    new
            //    {
            //        v.FirstName,
            //        v.LastName,
            //        v.Business
            //    }).Distinct().ToList();


            //ViewData["ReturningVisitors"] = ReturningVisitors.Distinct()
            //    .OrderBy(n => n.FirstName)
            //    .Select(n => new SelectListItem
            //    {
            //        Value = n.FirstName + "," + n.LastName + "," + n.Business,
            //        Text = n.FirstName + " " + n.LastName + " " + n.Business


            //    }).ToList();


            //loaded in the Dropdownbox
            ViewData["ReturningVisitors"] = _context.Visitor.Distinct()
                .OrderBy(n => n.FirstName)
                .Select(n => new SelectListItem
                {
                    Value = (n.Id + " " + n.StaffName.Id).ToString(),
                    Text = n.FirstName.Trim() + " " + n.LastName.Trim() + " " + n.Business.Trim() + " " + n.StaffName.Name.Trim()
                }).ToList();


            List<Visitor> VisitorLogOut = new List<Visitor>();

            VisitorLogOut.AddRange(_context.Visitor.OrderBy(v => v.FirstName).Where(v => v.DateOut == default(DateTime)).ToList());

            ViewData["VisitorLogOut"] = VisitorLogOut;

            return View();
        }

        private List<SelectListItem> ListOfStaff(List<SelectListItem> Top5Staff)
        {
            Top5Staff.AddRange(_dataBaseCalls.Top5StaffVisitors()
                .Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Name + " " + n.Department
                }).ToList());

            Top5Staff.AddRange(_context.StaffNames
                .Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Name + " " + n.Department
                }).ToList());

            return Top5Staff;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

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
                return Redirect("~/Home/Index");
            }

            return NotFound();
        }
    }
}
