using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegsandRegsCS.Data;

namespace LegsandRegsCS.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        String password = "MFFwmEE3ZZXicsCGcDY5ZoW91ff96IMYPnfeQaxA";
        [HttpGet("{password}")]
        public async Task<ActionResult<String>> UpdateDatabase(string password)
        {
            if (password.Equals(this.password))
                SeedData.testDbInput();
            else
                return "Password is not valid";

            return "Database update has been triggered";
        }
    }
}