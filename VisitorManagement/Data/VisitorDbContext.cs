using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VisitorManagement.Models;

namespace VisitorManagement.Data
{
    public class VisitorDbContext : DbContext
    {
        public DbSet<StaffNames> StaffNames { get; set; }
        public DbSet<Visitor> Visitor { get; set; }

        public VisitorDbContext(DbContextOptions<VisitorDbContext> options)
            : base(options)
        {
        }
        public VisitorDbContext()
        {
        }

        //https://stackoverflow.com/questions/50657268/no-database-provider-has-been-configured-for-this-dbcontext-using-sqlite

        //public VisitorDbContext CreateDbContext(string[] args)
        //{
        //    var builder = new DbContextOptionsBuilder<VisitorDbContext>();
        //    builder.UseSqlite("Data Source=VMan.db");

        //    return new VisitorDbContext(builder.Options);
        //}

        //removed when controllers were created as it throws an error
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("Data Source = VMan.db");
        //}



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        
        }
    }
}
