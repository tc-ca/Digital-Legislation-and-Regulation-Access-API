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
    
        [HttpGet]
        public async Task<ActionResult<String>> UpdateDatabase([FromHeader] string password)
        {
            if (password.Equals(Program.databaseUpdatePassword))
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
            if (password.Equals(Program.databaseUpdatePassword))
                SeedData.Update(true);
            else
                return "Password is not valid";

            Program.telemetry.TrackTrace("Update forced by API call");
            Program.telemetry.TrackEvent("DB_UPDATE_FORCED");

            return "Database force update has been triggered";
        }
    }
}