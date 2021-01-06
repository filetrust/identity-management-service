using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private ITokenService _tokenService;

        public UsersController(
            IUserService userService,
            ITokenService tokenService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values);

            var user = await _userService.AuthenticateAsync(model.Username, model.Password, cancellationToken);

            if (user == null) return BadRequest(new { message = "Username or password is incorrect" });

            var token = _tokenService.GetToken(user.Id.ToString());

            // return basic user info and authentication token
            return Ok(new
            {
                user.Id,
                user.Username,
                user.FirstName,
                user.LastName,
                token
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username
            };

            await _userService.CreateAsync(user, model.Password, cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var users = new List<UserModel>();

            await foreach (var x in _userService.GetAllAsync(cancellationToken))
            {
                users.Add(new UserModel
                {
                    FirstName = x.FirstName,
                    Id = x.Id,
                    LastName = x.LastName,
                    Username = x.Username
                });
            };

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);

            return Ok(new UserModel
            {
                Id = id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody][Required]UpdateModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values);

            await _userService.UpdateAsync(new User
            {
                Id = id,
                FirstName = model.FirstName,
                Username = model.Username,
                LastName = model.LastName
            }, model.Password, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _userService.DeleteAsync(id, cancellationToken);

            return Ok();
        }
    }
}
