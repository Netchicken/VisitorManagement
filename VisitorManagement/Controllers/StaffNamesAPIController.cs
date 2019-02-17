using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorManagement.Data;
using VisitorManagement.Models;

namespace VisitorManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffNamesAPIController : ControllerBase
    {
        private readonly VisitorDbContext _context;

        public StaffNamesAPIController(VisitorDbContext context)
        {
            _context = context;
        }

        // GET: api/StaffNamesAPI
        [HttpGet]
        public IEnumerable<StaffNames> GetStaffNames()
        {
            return _context.StaffNames;
        }

        // GET: api/StaffNamesAPI/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffNames([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var staffNames = await _context.StaffNames.FindAsync(id);

            if (staffNames == null)
            {
                return NotFound();
            }

            return Ok(staffNames);
        }

        // PUT: api/StaffNamesAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaffNames([FromRoute] int id, [FromBody] StaffNames staffNames)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != staffNames.Id)
            {
                return BadRequest();
            }

            _context.Entry(staffNames).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffNamesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/StaffNamesAPI
        [HttpPost]
        public async Task<IActionResult> PostStaffNames([FromBody] StaffNames staffNames)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.StaffNames.Add(staffNames);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStaffNames", new { id = staffNames.Id }, staffNames);
        }

        // DELETE: api/StaffNamesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaffNames([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var staffNames = await _context.StaffNames.FindAsync(id);
            if (staffNames == null)
            {
                return NotFound();
            }

            _context.StaffNames.Remove(staffNames);
            await _context.SaveChangesAsync();

            return Ok(staffNames);
        }

        private bool StaffNamesExists(int id)
        {
            return _context.StaffNames.Any(e => e.Id == id);
        }
    }
}