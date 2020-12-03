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
    public class LanguagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LanguagesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Languages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguage()
        {
            var headers = this.Request.Headers;

            if (headers.TryGetValue(Program.secretTokenHeader, out Program.secretToken) == false)
                return Unauthorized();

            return await _context.Language.ToListAsync();
        }

        private bool LanguageExists(string id)
        {
            return _context.Language.Any(e => e.langCode == id);
        }
    }
}
