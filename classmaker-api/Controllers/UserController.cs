using System.Threading.Tasks;
using classmaker_models.Dtos;
using classmaker_models.Entities;
using classmaker_repository.Repositories;
using classmaker_services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace classmaker_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        
        public UserController(IUserService userService,
            IUserRepository userRepository)
        {
            _userService = userService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Validate/login an existing user
        /// </summary>
        /// <param name="user">Object with username and password</param>
        /// <returns>Result with error or jwt token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] AuthUserDto user)
        {
            
            if (!ModelState.IsValid || 
                string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest();
            }
            
            var result = await _userService.Authenticate(user.Username, user.Password);

            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="user">Object with username and password</param>
        /// <returns>Result with error or jwt token</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] AuthUserDto user)
        {
            if (!ModelState.IsValid 
                || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest();
            }

            var createResult = await _userRepository.Create(new User
            {
                Username = user.Username
            }, user.Password);

            if (!createResult.IsSuccess)
            {
                return BadRequest(createResult);
            }

            var result = await _userService.Authenticate(user.Username, user.Password);
            
            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete a user by iD
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Result object with success or error</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteById(int id)
        {
            return Ok(await _userRepository.DeleteById(id));
        }
        
        /// <summary>
        /// Delete a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Result object with success or error</returns>
        [HttpDelete("{username}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest();
            }
            return Ok(await _userRepository.DeleteByUsername(username));
        }
    }
}