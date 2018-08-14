using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using VisitorManagement.Data;
using VisitorManagement.DTO;
using VisitorManagement.Models;

namespace VisitorManagement.Business
{
    public class TextFileOperations : ITextFileOperations
    {
        private readonly VisitorDbContext _context;
        private readonly IHostingEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TextFileOperations(VisitorDbContext context, IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public void UploadStaffFile()
        {

            //--------------Get the data from the Text file --------------
            string[] lines = { };

            //Gets or sets the absolute path to the directory that contains the web-servable application content files.
            string webRootPath = _environment.WebRootPath;

            FileInfo filePath = new FileInfo(Path.Combine(webRootPath, "Staff.txt"));
            lines = System.IO.File.ReadAllLines(filePath.ToString());

            //holds all the staff names
            Dictionary<string, StaffNames> StaffnamesDict = new Dictionary<string, StaffNames>();

            List<StaffNames> AllStaffNames = new List<StaffNames>();

            foreach (var name in lines)
            {
                StaffNames staffnames = new StaffNames();
                string[] data = name.Split(',');
                staffnames.Name = data[0];
                staffnames.Department = data[1];

                //if the name isn't there, add it in
                if (!StaffnamesDict.ContainsKey(staffnames.Name))
                {
                    StaffnamesDict.Add(staffnames.Name, staffnames);
                    AllStaffNames.Add(staffnames);
                }
            }


            //--------------Get the data from the Database --------------

            //add in the ones from the db
            AllStaffNames.AddRange(RemoveDuplicateStaff());
            //sort the new data and pull out only uniques
            IOrderedEnumerable<StaffNames> orderedStaffNames = AllStaffNames.Distinct().OrderBy(
                i => i.Department).ThenBy(i => i.Name);
            _context.AddRange(orderedStaffNames);
            _context.SaveChanges();
        }



        public IEnumerable<StaffNames> RemoveDuplicateStaff()
        {
            //holds all the staff names
            Dictionary<string, StaffNames> StaffnamesDict = new Dictionary<string, StaffNames>();
            List<StaffNames> AllStaffNames = new List<StaffNames>();

            foreach (var name in _context.StaffNames.Distinct().ToList())
            //.Select(x => new { x.Name, x.Department })
            {
                //have to remove 
                StaffNames staffnames = new StaffNames();
                staffnames.Name = name.Name;
                staffnames.Department = name.Department;
                staffnames.Id = name.Id;
                if (!StaffnamesDict.ContainsKey(staffnames.Name))
                {
                    StaffnamesDict.Add(staffnames.Name, staffnames);
                    AllStaffNames.Add(staffnames);
                }
            }

            //Empty the Database to remove old values

            foreach (var staff in _context.StaffNames.ToList())
            {
                _context.StaffNames.Remove(staff);
            }
            _context.SaveChanges();

            //save the filtered data
            //_context.AddRange(AllStaffNames);
            //_context.SaveChanges();


            return AllStaffNames;
        }


        public IEnumerable<string> LoadConditionsForAcceptanceText()
        {
            string[] lines = { };

            //Gets or sets the absolute path to the directory that contains the web-servable application content files.
            string webRootPath = _environment.WebRootPath;
            //Gets or sets the absolute path to the directory that contains the application content files.
        //    string contentRootPath = _environment.ContentRootPath;

            FileInfo filePath = new FileInfo(Path.Combine(webRootPath, "ConditionsForAdmittance.txt"));

            lines = File.ReadAllLines(filePath.ToString());

            return lines.ToList();
        }
        //---- Downloading and backup of files
        //https://stackoverflow.com/questions/44432294/download-file-using-asp-net-core

        public void DownloadVisitorsExcel(string filePath, string fileName)
        {

            IEnumerable<Visitor> AllVisitors = (IEnumerable<Visitor>)_context.Visitor.Find();


            var sb = new StringBuilder();
            sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}", "First Name", "Last Name", "Business", "Date In", "Date Out", "Staff Name", "Department", Environment.NewLine);
            foreach (var item in AllVisitors)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}", item.FirstName, item.LastName, item.Business, item.DateIn.ToShortDateString() + item.DateIn.ToShortTimeString(), item.DateOut.ToShortDateString() + item.DateOut.ToShortTimeString(), item.StaffName.Name, item.StaffName.Department, Environment.NewLine);
            }


            ////Get Current Response  
            //var response = _httpContextAccessor.HttpContext.Response;
            ////response.BufferOutput = true;
            //response.Clear();
            ////response.ClearHeaders();
            ////response.ContentEncoding = Encoding.Unicode;
            //response.Body.    AddHeader("content-disposition", "attachment;filename=Visitors.CSV ");
            //response.ContentType = "text/plain";
            //response.Body.Write(sb.ToString());
            //response.Body.EndWrite();



            IFileProvider provider = new PhysicalFileProvider(filePath);
            IFileInfo fileInfo = provider.GetFileInfo(fileName);
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "text/plain";
            //  return FileMode(readStream, mimeType, fileName);


        }

    }
}
