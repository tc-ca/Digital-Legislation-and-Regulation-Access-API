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
        private readonly string password = "pxXu84wwyRAgudn5JT3KWrESZRzXBEmV";

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

        // PUT: api/Languages/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("Modify/{password}")]
        public async Task<IActionResult> PutLanguage(string password, Language language)
        {
            if (password != this.password)
            {
                return Unauthorized();
            }

            _context.Entry(language).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(language.langCode))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Languages
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("Create/{password}")]
        public async Task<ActionResult<Language>> PostLanguage(string password, Language language)
        {
            if (password != this.password)
            {
                return Unauthorized();
            }

            _context.Language.Add(language);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LanguageExists(language.langCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLanguage", new { id = language.langCode }, language);
        }



        private bool LanguageExists(string id)
        {
            return _context.Language.Any(e => e.langCode == id);
        }
    }
}
