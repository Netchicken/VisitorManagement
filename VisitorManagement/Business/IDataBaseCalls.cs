using System.Collections.Generic;
using VisitorManagement.Models;

namespace VisitorManagement.Business
{
    public interface IDataBaseCalls
    {
        IEnumerable<StaffNames> Top5StaffVisitors();
        IEnumerable<Visitor> UniqueVisitorNames();
        IEnumerable<Visitor> VisitorsLoggedIn();
        void IncrementStaffCount(int Id);
    }
}