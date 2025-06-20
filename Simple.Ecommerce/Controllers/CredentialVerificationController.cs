using Simple.Ecommerce.App.Interfaces.Commands.CredentialVerificationCommands;
using Simple.Ecommerce.Contracts.CredentialVerificationContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CredentialVerificationController : ControllerBase
    {
        private readonly ICreateCredentialVerificationCommand _createCredentialVerificationCommand;
        private readonly IConfirmCredentialVerificationCommand _confirmCredentialVerificationCommand;

        public CredentialVerificationController(
            ICreateCredentialVerificationCommand createCredentialVerificationCommand, 
            IConfirmCredentialVerificationCommand confirmCredentialVerificationCommand
        )
        {
            _createCredentialVerificationCommand = createCredentialVerificationCommand;
            _confirmCredentialVerificationCommand = confirmCredentialVerificationCommand;
        }

        [HttpPost("Create/{loginId}")]
        [Authorize]
        public async Task<ActionResult<CredentialVerificationResponse>> Create(int loginId)
        {
            var result = await _createCredentialVerificationCommand.Execute(loginId);

            if (result.IsFailure)
            {
                var message = "";

                foreach (var error in result.Errors!)
                {
                    message += error.Type + ": " + error.Message;
                }

                return Problem(message);
            }

            return Ok(result.GetValue());
        }

        [HttpPut("Confirm/{token}")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> Confirm(string token)
        {
            var result = await _confirmCredentialVerificationCommand.Execute(token);

            if (result.IsFailure)
            {
                var message = "";

                foreach (var error in result.Errors!)
                {
                    message += error.Type + ": " + error.Message;
                }

                return Problem(message);
            }

            return Ok(result.GetValue());
        }
    }
}
