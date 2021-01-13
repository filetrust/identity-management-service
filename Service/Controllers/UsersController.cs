using Glasswall.IdentityManagementService.Api.ActionFilters;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Glasswall.IdentityManagementService.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(ModelStateValidationActionFilterAttribute))]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IIdentityManagementServiceConfiguration _identityManagementServiceConfiguration;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public UsersController(
            IIdentityManagementServiceConfiguration identityManagementServiceConfiguration,
            IUserService userService,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _identityManagementServiceConfiguration = identityManagementServiceConfiguration ?? throw new ArgumentNullException(nameof(identityManagementServiceConfiguration));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values);

            var user = await _userService.AuthenticateAsync(model.Username, model.Password, cancellationToken);

            if (user == null) return Unauthorized(new { message = "Username or password is incorrect" });

            var token = _tokenService.GetToken(user.Id.ToString(), _identityManagementServiceConfiguration.TokenSecret, _identityManagementServiceConfiguration.TokenLifetime);

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
        [HttpPost("new")]
        public async Task<IActionResult> New([FromBody]RegisterModel model, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();

            var createdUser = await _userService.CreateAsync(new User
            {
                Id = id,
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            }, cancellationToken);

            var confirmEmailToken = _tokenService.GetToken(
                createdUser.Id.ToString(), 
                string.Join(null, createdUser.PasswordHash),
                _identityManagementServiceConfiguration.TokenLifetime);

            await _emailService.SendAsync(new NewUserEmail(createdUser, _identityManagementServiceConfiguration, confirmEmailToken), cancellationToken);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("reset")]
        public async Task<IActionResult> Reset([FromQuery] string username, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values);

            User associatedUser = null;

            await foreach (var user in _userService.GetAllAsync(cancellationToken))
            {
                if (user.Username == username)
                    associatedUser = user;
            }

            if (associatedUser == null)
                return BadRequest(new { message = $"{username} was not found" });

            var resetToken = _tokenService.GetToken(
                associatedUser.Id.ToString(),
                string.Join(null, associatedUser.PasswordHash),
                _identityManagementServiceConfiguration.TokenLifetime
            );

            await _emailService.SendAsync(new ResetPasswordEmail(associatedUser, _identityManagementServiceConfiguration, resetToken), cancellationToken);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var users = new List<object>();

            await foreach (var x in _userService.GetAllAsync(cancellationToken))
            {
                users.Add(new 
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Username,
                    x.Email,
                    x.Status
                });
            };

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);

            if (user == null)
                return BadRequest(new { message = "User does not exist" });

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Username,
                user.Email,
                user.Status
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
