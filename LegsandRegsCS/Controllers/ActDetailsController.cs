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
    public class ActDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ActDetails/5
        [HttpGet("{uniqueId}/{lang}")]
        [ProducesResponseType(typeof(ActDetails), 200)]
        public async Task<ActionResult<string>> GetActDetails(string uniqueId, string lang)
        {
            var actDetail = await _context.ActDetails.FindAsync(uniqueId,lang);

            if (actDetail == null)
            {
                return NotFound();
            }

            //We must split up the ActDetails object and add it back together so that the details get presented as objects and not just strings (since they are stored as a string)
            JToken fullDetails = JToken.Parse(actDetail.fullDetails);

            JObject parsed = JObject.Parse(
                JsonConvert.SerializeObject(
                    new ActId
                    {
                        uniqueId = actDetail.uniqueId,
                        lang = actDetail.lang
                    }));

            parsed.Add("fullDetails", fullDetails);

            return parsed.ToString();
            
        }

        // POST: api/ActDetails
        [HttpPost]
        [ProducesResponseType(typeof(List<ActDetails>), 200)]
        public async Task<ActionResult<string>> GetActs([FromBody] List<ActId> ids)
        {
            if (ids == null)
            {
                return NotFound();
            }

            List<string> concatIds = new List<string>();

            foreach (ActId id in ids)
            {
                concatIds.Add(id.uniqueId + id.lang);
            }

            List<JObject> output = new List<JObject>();


            var actDetails = await _context.ActDetails.Where(a => concatIds.Contains(a.uniqueId + a.lang)).ToListAsync();
            foreach(var actDetail in actDetails)
            {
                JToken fullDetails = JToken.Parse(actDetail.fullDetails);

                JObject parsed = JObject.Parse(
                    JsonConvert.SerializeObject(
                        new ActId
                        {
                            uniqueId = actDetail.uniqueId,
                            lang = actDetail.lang
                        }));

                parsed.Add("fullDetails", fullDetails);
                output.Add(parsed);

            }
            return JsonConvert.SerializeObject(output);
        }

        private bool ActDetailsExists(string id)
        {
            return _context.ActDetails.Any(e => e.uniqueId == id);
        }
    }
}
