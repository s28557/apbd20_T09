using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using T09.Data;
using T09.Models;

namespace T09.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly ApbdContext _context;
    public TripController(ApbdContext context)
    {
        _context = context;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {

        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Page and pageSize parameters must be above 0");
        }

        var trips = await _context.Trips
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .OrderByDescending(t => t.DateFrom)
            .ToListAsync();

        return Ok(trips);
    }
    
    /*
    public async Task<IActionResult> GetTrips()
    {
        //var trips = _context.Trips.ToList();
        //var trips = await _context.Trips.ToListAsync(); // pobralo wszystko
        
        var trips = await _context.Trips.Select(
            e => new
            {
                Name = e.Name,
                Countries = e.IdCountries.Select(
                    c => new
                    {
                        Name = c.Name
                    })
            })
            .ToListAsync();   // tylko name
        
        return Ok(trips);
    }
    */
}