using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LegsandRegsCS.Data;
using LegsandRegsCS.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
        [ProducesResponseType(typeof(List<ActSummary>), 200)]
        public async Task<ActionResult<string>> GetActs()
        {
            var headers = this.Request.Headers;

            if (headers.TryGetValue(Program.secretTokenHeader, out Program.secretToken) == false)
                return Unauthorized();
            if (Program.downForMaintenance)
                return StatusCode(503);

            var acts = await _context.Act.ToListAsync();

            List<JObject> output = new List<JObject>();

            foreach (var act in acts)
            {
                JObject parsed = JObject.Parse(
                    JsonConvert.SerializeObject(act));

                parsed.Remove("regs");

                output.Add(parsed);
            }

            return JsonConvert.SerializeObject(output);
        }

        // GET: api/Acts/Regs
        [HttpGet("Regs")]
        public async Task<ActionResult<IEnumerable<Act>>> GetActsWithRegs()
        {
            var headers = this.Request.Headers;

            if (headers.TryGetValue(Program.secretTokenHeader, out Program.secretToken) == false)
                return Unauthorized();
            if (Program.downForMaintenance)
                return StatusCode(503);

            return await _context.Act.Include(x => x.regs).ThenInclude(x => x.reg).ToListAsync();

        }

        // GET: api/Acts/5
        [HttpGet("{uniqueId}/{lang}")]
        public async Task<ActionResult<Act>> GetAct(string uniqueId, string lang)
        {
            var headers = this.Request.Headers;

            if (headers.TryGetValue(Program.secretTokenHeader, out Program.secretToken) == false)
                return Unauthorized();
            if (Program.downForMaintenance)
                return StatusCode(503);

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
            var headers = this.Request.Headers;

            if (headers.TryGetValue(Program.secretTokenHeader, out Program.secretToken) == false)
                return Unauthorized();
            if (Program.downForMaintenance)
                return StatusCode(503);

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
