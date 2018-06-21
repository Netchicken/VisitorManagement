using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VisitorManagement.Data;
using VisitorManagement.Models;

namespace VisitorManagement.Business
{
    public class DataBaseCalls : IDataBaseCalls
    {
        private readonly VisitorDbContext _context;
        public DataBaseCalls(VisitorDbContext context)
        {
            _context = context;
        }

        public IEnumerable<StaffNames> Top5StaffVisitors()
        {
            //get a list of all the popular staff
            List<StaffNames> PopularStaff = new List<StaffNames>();

            IEnumerable<StaffNames> AllStaff = _context.StaffNames.Distinct()
                .OrderBy(i => i.Department)
                .ThenBy(i => i.Name)
                .ToList();

            //where the count is over 0 and take the top 5
            foreach (var staff in AllStaff.
                OrderByDescending(s => s.VisitorCount > 0)
                .Take(5))
            {
                if (staff.VisitorCount > 0)
                {
                    PopularStaff.Add(staff);
                }
            }

            return PopularStaff.ToList();
        }

        public IEnumerable<Visitor> UniqueVisitorNames()
        {
            return _context.Visitor.Distinct().OrderBy(v => v.FirstName).ToList();
        }
        public IEnumerable<Visitor> VisitorsLoggedIn()
        {
            return _context.Visitor.OrderBy(v => v.FirstName).Where(v => v.DateOut == default(DateTime)).ToList();
        }

        public void IncrementStaffCount(int id)
        {
            var MostRecentStaffVisited = (StaffNames) _context.StaffNames.Find(id);
            //_context.Visitor.OrderByDescending(u => u.Id).FirstOrDefault();
            //Debug.Assert(MostRecentVisitor != null, nameof(MostRecentVisitor) + " != null");
            MostRecentStaffVisited.VisitorCount++;
            _context.Update(MostRecentStaffVisited);
            _context.SaveChangesAsync();
            //
            //
            //
            // _context.StaffNames.OrderByDescending(u => u.Id).FirstOrDefault();
            //  staffNames.VisitorCount++;

        }
    }
}
