using AutoMapper;
using HotelLstWebApi.Data;
using HotelLstWebApi.Models;
using HotelLstWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelLstWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUsers> _userManager;
        //private readonly SignInManager<ApiUsers> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManger;
        public AccountController(UserManager<ApiUsers>userManager,
            //SignInManager<ApiUsers> signInManager,
            ILogger<AccountController> logger, 
            IMapper mapper, IAuthManager authManger)
        {
            _userManager = userManager;
            //_signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _authManger = authManger;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registration attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = _mapper.Map<ApiUsers>(userDTO);
                user.UserName = user.Email;
                var result = await _userManager.CreateAsync(user,userDTO.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                await _userManager.AddToRolesAsync(user, userDTO.Roles);
                return Accepted();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Something went wrong in the {nameof(Register)}");
                return Problem($"Something went wrong in the {nameof(Register)}",statusCode:500);
            }            
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login([FromBody] LoginUserDTO userdto)
        {
            _logger.LogInformation($"login attempt for {userdto.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                
                if (! await _authManger.ValidateUser(userdto))
                {
                    return Unauthorized();
                }
                return Accepted(new {Token=await _authManger.CreateToken() });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"something went wrong in the {nameof(Register)}");
                return Problem($"something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }
    }
}
