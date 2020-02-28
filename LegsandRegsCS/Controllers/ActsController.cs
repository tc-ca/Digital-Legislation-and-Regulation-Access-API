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
    public class ActsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Acts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Act>>> GetAct()
        {
            return await _context.Act.ToListAsync();
        }

        // GET: api/Acts/5
        [HttpGet("{uniqueId}/{lang}")]
        public async Task<ActionResult<Act>> GetAct(string uniqueId, string lang)
        {
            var act = await _context.Act.FindAsync(uniqueId,lang);

            if (act == null)
            {
                return NotFound();
            }

            return act;
        }

        private async Task<ActionResult<Act>> PostAct(Act act)
        {
            _context.Act.Add(act);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ActExists(act.uniqueId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAct", new { id = act.uniqueId }, act);
        }

        private async Task<ActionResult<Act>> DeleteAct(string id)
        {
            var act = await _context.Act.FindAsync(id);
            if (act == null)
            {
                return NotFound();
            }

            _context.Act.Remove(act);
            await _context.SaveChangesAsync();

            return act;
        }

        private bool ActExists(string id)
        {
            return _context.Act.Any(e => e.uniqueId == id);
        }
    }
}
