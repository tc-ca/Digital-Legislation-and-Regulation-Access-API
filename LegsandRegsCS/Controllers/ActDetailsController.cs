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
    public class ActDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ActDetails/5
        [HttpGet("{uniqueId}/{lang}")]
        public async Task<ActionResult<ActDetails>> GetActDetails(string uniqueId, string lang)
        {
            var actDetails = await _context.ActDetails.FindAsync(uniqueId,lang);

            if (actDetails == null)
            {
                return NotFound();
            }

            return actDetails;
        }

        private bool ActDetailsExists(string id)
        {
            return _context.ActDetails.Any(e => e.uniqueId == id);
        }
    }
}
