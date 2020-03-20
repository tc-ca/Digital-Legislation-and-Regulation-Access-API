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
        public async Task<ActionResult<string>> GetRegDetail(string id)
        {
            var regDetails = await _context.RegDetails.FindAsync(id);

            if (regDetails == null)
            {
                return NotFound();
            }

            return JsonConvert.SerializeObject(regDetails).Replace(@"\", "");
        }

        [HttpPost]
        public async Task<ActionResult<string>> GetRegdetails([FromBody] string[] ids)
        {
            if (ids == null)
            {
                return NotFound();
            }

            var regDetails = await _context.RegDetails.Where(r => ids.Contains(r.id)).ToListAsync();
            return JsonConvert.SerializeObject(regDetails).Replace(@"\", "");
        }

        private bool RegDetailsExists(string id)
        {
            return _context.RegDetails.Any(e => e.id == id);
        }
    }
}
