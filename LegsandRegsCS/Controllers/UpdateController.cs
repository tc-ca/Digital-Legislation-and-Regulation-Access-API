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

        private readonly AppDbContext _context;

        public UpdateController(AppDbContext context)
        {
            _context = context;
        }
    
        String password = "MFFwmEE3ZZXicsCGcDY5ZoW91ff96IMYPnfeQaxA";
        [HttpGet]
        public async Task<ActionResult<String>> UpdateDatabase([FromHeader] string password)
        {
            if (password.Equals(this.password))
                SeedData.Update();
            else
                return "Password is not valid";

            Program.telemetry.TrackTrace("Update requested by API call");
            Program.telemetry.TrackEvent("DB_UPDATE_REQUESTED");

            return "Database update has been triggered";
        }
        [HttpGet("Force")]
        public async Task<ActionResult<String>> ForceUpdateDatabase([FromHeader] string password)
        {
            if (password.Equals(this.password))
                SeedData.Update(true);
            else
                return "Password is not valid";

            Program.telemetry.TrackTrace("Update forced by API call");
            Program.telemetry.TrackEvent("DB_UPDATE_FORCED");

            return "Database force update has been triggered";
        }
    }
}