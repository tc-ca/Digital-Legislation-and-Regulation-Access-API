using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LegsandRegsCS.Controllers
{
    [Route("getActs")]
    [ApiController]
    public class ActsController : ControllerBase
    {
        // GET: api/Acts
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Acts/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value" + id as String;
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
