using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorManagement.Models;

namespace VisitorManagement.Business
{
    public interface ITextFileOperations
    {
        void UploadStaffFile();
        IEnumerable<string> LoadConditionsForAcceptanceText();
        IEnumerable<StaffNames> RemoveDuplicateStaff();
    }
}
