using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegsandRegsCS.Special_Data_Types;
using System.Text.Json;

namespace LegsandRegsCS.Controllers
{
    [Route("api/acts")]
    [ApiController]
    public class ActsController : ControllerBase
    {
        // GET: api/Acts
        [HttpGet]
        public String Get()
        {
            return JsonSerializer.Serialize(DatabaseConnector.getActs());
        }

        // GET: api/Acts/5
        [HttpGet("{uniqueId}/{lang}", Name = "Get")]
        public string Get(string uniqueId, string lang)
        {
            return JsonSerializer.Serialize(DatabaseConnector.getActById(uniqueId, lang));
        }

        // POST: api/Acts
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Acts/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
