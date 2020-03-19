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
            return await _context.Act.Include(x => x.regs).ToListAsync();
        }

        // GET: api/Acts/5
        [HttpGet("{uniqueId}/{lang}")]
        public async Task<ActionResult<Act>> GetAct(string uniqueId, string lang)
        {
            var act = await _context.Act.Include(x => x.regs).FirstOrDefaultAsync(x => x.uniqueId == uniqueId && x.lang == lang);

            if (act == null)
            {
                return NotFound();
            }

            return act;
        }

        // POST: api/Acts
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Act>>> GetActs([FromBody] List<ActId> ids)
        {
            if (ids == null)
            {
                return NotFound();
            }

            List<string> concatIds = new List<string>();

            foreach(ActId id in ids)
            {
                concatIds.Add(id.uniqueId + id.lang);
            }

            return await _context.Act.Where(a => concatIds.Contains(a.uniqueId + a.lang)).ToListAsync();
        }

        private bool ActExists(string id)
        {
            return _context.Act.Any(e => e.uniqueId == id);
        }
    }
}
