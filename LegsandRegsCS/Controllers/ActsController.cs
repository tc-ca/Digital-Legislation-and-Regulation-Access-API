using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegsandRegsCS.Special_Data_Types;

namespace LegsandRegsCS.Controllers
{
    [Route("api/acts")]
    [ApiController]
    public class ActsController : ControllerBase
    {
        // GET: api/Acts
        [HttpGet]
        public IEnumerable<Act> Get()
        {
            Act act1 = new Act("A1", "A1", "example title blablabla", "eng", "2020-04-28");
            Act act2 = new Act("A2", "A2", "example title2 blablabla", "fra", "2020-07-19");
            List<Act> list = new List<Act>();
            list.Add(act1);
            list.Add(act2);
            IEnumerable<Act> iEnumerable = list;
            return iEnumerable;
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
