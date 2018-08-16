using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace VisitorManagement.Business
{
    public class VisitorComparer : IEqualityComparer<SelectListItem>
    {
        public bool Equals(SelectListItem x, SelectListItem y)
        {
            //if (Object.ReferenceEquals(x, y)) return true;

            //if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            //    return false;

            return x.Text == y.Text;
        }

        public int GetHashCode(SelectListItem obj)
        {
            //if (Object.ReferenceEquals(obj, null)) return 0;
            //int hashVisitorText = obj.Text == null ? 0 : obj.Text.GetHashCode();
            //int hashVisitorValue = obj.Value.GetHashCode();
            //return hashVisitorText ^ hashVisitorValue;
            return obj.Text.GetHashCode();
        }

    }
}
