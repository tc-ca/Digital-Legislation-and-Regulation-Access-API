using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LegsandRegsCS.Data;
using LegsandRegsCS.Models;

namespace LegsandRegsCS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Regs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reg>>> GetReg()
        {
            return await _context.Reg.ToListAsync();
        }

        // GET: api/Regs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reg>> GetReg(string id)
        {
            var reg = await _context.Reg.FindAsync(id);

            if (reg == null)
            {
                return NotFound();
            }

            return reg;
        }

        private async Task<ActionResult<Reg>> PostReg(Reg reg)
        {
            _context.Reg.Add(reg);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RegExists(reg.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetReg", new { id = reg.id }, reg);
        }

        private async Task<ActionResult<Reg>> DeleteReg(string id)
        {
            var reg = await _context.Reg.FindAsync(id);
            if (reg == null)
            {
                return NotFound();
            }

            _context.Reg.Remove(reg);
            await _context.SaveChangesAsync();

            return reg;
        }

        private bool RegExists(string id)
        {
            return _context.Reg.Any(e => e.id == id);
        }
    }
}
