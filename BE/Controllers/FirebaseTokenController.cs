using BE.Data.Contexts;
using BE.Data.Dtos.Firebase_TokenDtos;
using BE.Data.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirebaseTokenController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FirebaseTokenController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Firebase_Token
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Firebase_Token>>> GetFirebase_Token()
        {
            return await _context.Firebase_Token.ToListAsync();
        }

        // GET: Firebase_Token/5
        [HttpGet("{username}")]
        public async Task<ActionResult<Firebase_Token>> GetFirebaseToken(string username)
        {
            var firebaseToken = await _context.Firebase_Token.FirstOrDefaultAsync(x => x.UserName == username);

            if (firebaseToken == null)
            {
                return NotFound(username);
            }

            return firebaseToken;
        }

        // PUT: Firebase_Token/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFirebaseToken(int id, Firebase_Token firebaseToken)
        {
            if (id != firebaseToken.Id)
            {
                return BadRequest();
            }

            _context.Entry(firebaseToken).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FirebaseTokenExists(id))
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

        // POST: Firebase_Token
        [HttpPost]
        public async Task<ActionResult<Firebase_Token>> PostFirebaseToken(Firebase_TokenDTO firebaseTokenDto)
        {
            var firebaseToken = new Firebase_Token
            {
                UserName = firebaseTokenDto.UserName,
                Token = firebaseTokenDto.Token
            };

            _context.Firebase_Token.Add(firebaseToken);
            await _context.SaveChangesAsync();
            return Ok(firebaseToken);

            //return CreatedAtAction("GetFirebaseToken", new { id = firebaseToken.Id }, firebaseToken);
        }

        // DELETE: Firebase_Token/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFirebaseToken(int id)
        {
            var firebaseToken = await _context.Firebase_Token.FindAsync(id);
            if (firebaseToken == null)
            {
                return NotFound();
            }

            _context.Firebase_Token.Remove(firebaseToken);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FirebaseTokenExists(int id)
        {
            return _context.Firebase_Token.Any(e => e.Id == id);
        }
    }

}
