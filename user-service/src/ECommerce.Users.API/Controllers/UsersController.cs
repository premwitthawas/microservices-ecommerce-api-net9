using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Users.Core.DTOs;
using ECommerce.Users.Core.ServiceContacts;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Users.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserDto>> GetUserByUserID(Guid userId){
            if(userId == Guid.Empty){
                return BadRequest("Invalid User ID");
            }
            UserDto? user = await _userService.GetUserByUserID(userId);
            if(user == null){
                return NotFound(user);
            }
            return Ok(user);
        }
    }
}