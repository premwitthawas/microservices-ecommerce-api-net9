using ECommerce.Users.Core.DTOs;
using ECommerce.Users.Core.ServiceContacts;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Users.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public AuthController(IUsersService usersService)
        {
            _usersService = usersService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null)
            {
                return BadRequest("Invalid Rigister Request");
            }
            AuthenticationResponse? res = await _usersService.RegisterAsync(registerRequest);
            if (res == null || res.Success == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest("Invalid Login Request");
            }
            AuthenticationResponse? res = await _usersService.Login(loginRequest);
            if (res == null || res.Success == false)
            {
                return Unauthorized(res);
            }
            return Ok(res);
        }
    }
}