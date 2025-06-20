using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.LoginCommands;
using Simple.Ecommerce.Contracts.LoginContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ICreateLoginCommand _createLoginCommand;
        private readonly IDeleteLoginCommand _deleteLoginCommand;
        private readonly IAuthenticateLoginCommand _authenticateLoginCommand;

        public LoginController(
            ICreateLoginCommand createLoginCommand, 
            IDeleteLoginCommand deleteLoginCommand, 
            IAuthenticateLoginCommand authenticateLoginCommand
        )
        {
            _createLoginCommand = createLoginCommand;
            _deleteLoginCommand = deleteLoginCommand;
            _authenticateLoginCommand = authenticateLoginCommand;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<bool>> Post([FromBody] LoginRequest request)
        {
            var result = await _createLoginCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _deleteLoginCommand.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest request)
        {
            var result = await _authenticateLoginCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }
    }
}
