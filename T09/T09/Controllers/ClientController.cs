using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T09.Data;
using T09.Models;

namespace T09.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApbdContext _context;

        public ClientsController(ApbdContext context)
        {
            _context = context;
        }

        
        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var client = await _context.Clients
                .Include(c => c.ClientTrips)
                .FirstOrDefaultAsync(c => c.IdClient == idClient);

            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            if (client.ClientTrips.Any())
            {
                return BadRequest(new { message = "Client has assigned trips" });
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        
        [HttpPost("trips/{idTrip}/clients")]
        public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] ClientRequest request)
        {
            if (await _context.Clients.AnyAsync(c => c.Pesel == request.Pesel))
            {
                return BadRequest(new { message = "Client with this PESEL already exists" });
            }

            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);

            if (trip == null)
            {
                return NotFound(new { message = "Trip not found" });
            }

            if (trip.DateFrom <= DateTime.Now)
            {
                return BadRequest(new { message = "Cannot register for a trip that has already started or ended" });
            }

            var client = new Client
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Telephone = request.Telephone,
                Pesel = request.Pesel
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var clientTrip = new ClientTrip
            {
                IdClient = client.IdClient,
                IdTrip = idTrip,
                RegisteredAt = DateTime.Now,
                PaymentDate = request.PaymentDate
            };

            _context.ClientTrips.Add(clientTrip);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Client registered for trip" });
        }
    }

    public class ClientRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Pesel { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
