using Microsoft.AspNetCore.Http;

namespace VisitorManagement.Models
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
