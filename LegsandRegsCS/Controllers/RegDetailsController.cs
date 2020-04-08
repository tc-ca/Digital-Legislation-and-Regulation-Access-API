using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LegsandRegsCS.Data;
using LegsandRegsCS.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LegsandRegsCS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RegDetails/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RegDetails), 200)]
        public async Task<ActionResult<string>> GetRegDetail(string id)
        {
            var regDetail = await _context.RegDetails.FindAsync(id);

            if (regDetail == null)
            {
                return NotFound();
            }

            JToken fullDetails = JToken.Parse(regDetail.fullDetails);

            JObject parsed = JObject.Parse(JsonConvert.SerializeObject(new RegIdWrapper { id = id }));
            parsed.Add("fullDetails",fullDetails);

            return parsed.ToString();
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<RegDetails>), 200)]
        public async Task<ActionResult<string>> GetRegdetails([FromBody] string[] ids)
        {
            if (ids == null)
            {
                return NotFound();
            }

            var regDetails = await _context.RegDetails.Where(r => ids.Contains(r.id)).ToListAsync();

            List<JObject> output = new List<JObject>();

            foreach (var regDetail in regDetails)
            {
                JToken fullDetails = JToken.Parse(regDetail.fullDetails);

                JObject parsed = JObject.Parse(JsonConvert.SerializeObject(new RegIdWrapper { id = regDetail.id }));
                parsed.Add("fullDetails", fullDetails);

                output.Add(parsed);
            }
            return JsonConvert.SerializeObject(output);
        }

        private bool RegDetailsExists(string id)
        {
            return _context.RegDetails.Any(e => e.id == id);
        }
    }
}
