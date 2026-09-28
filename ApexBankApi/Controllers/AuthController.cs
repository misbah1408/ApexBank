using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApexBankApi.Models;
using ApexBankApi.Data;
using ApexBankApi.Dtos;
namespace ApexBankApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // GET: api/User
    [HttpPost("getuser")]
    public async Task<ActionResult<User>> GetUser([FromBody] UserDto u)
    {
        if (u == null)
            return BadRequest();

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email == u.Email &&
                x.Password == u.Password &&
                x.IsActive &&
                x.IsApproved);

        if (user == null)
            return Unauthorized();

        return Ok(user);
    }
}
