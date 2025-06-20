using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.Contracts.UserAddressContracts;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Contracts.UserPhotoContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ICreateUserCommand _createUserCommand;
        private readonly IDeleteUserCommand _deleteUserCommand;
        private readonly IUpdateUserCommand _updateUserCommand;
        private readonly IGetUserQuery _getUserQuery;
        private readonly IListUserQuery _listUserQuery;
        private readonly IAddAddressUserCommand _addAddressUserCommand;
        private readonly IAddPhotoUserCommand _addPhotoUserCommand;
        private readonly IDeleteAddressUserCommand _deleteAddressUserCommand;
        private readonly IDeletePhotoUserCommand _deletePhotoUserCommand;
        private readonly IGetAddressesUserQuery _getAddressesUserQuery;
        private readonly IGetPhotoUserQuery _getPhotoUserQuery;

        public UserController(
            ICreateUserCommand createUserCommand, 
            IDeleteUserCommand deleteUserCommand, 
            IUpdateUserCommand updateUserCommand,
            IGetUserQuery getUserQuery,
            IListUserQuery listUserQuery,
            IAddAddressUserCommand addAddressUserCommand, 
            IAddPhotoUserCommand addPhotoUserCommand,
            IDeleteAddressUserCommand deleteAddressUserCommand,
            IDeletePhotoUserCommand deletePhotoUserCommand,
            IGetAddressesUserQuery getAddressesUserQuery, 
            IGetPhotoUserQuery getPhotoUserQuery
        )
        {
            _createUserCommand = createUserCommand;
            _deleteUserCommand = deleteUserCommand;
            _updateUserCommand = updateUserCommand;
            _getUserQuery = getUserQuery;
            _listUserQuery = listUserQuery;
            _addAddressUserCommand = addAddressUserCommand;
            _addPhotoUserCommand = addPhotoUserCommand;
            _deleteAddressUserCommand = deleteAddressUserCommand;
            _deletePhotoUserCommand = deletePhotoUserCommand;
            _getAddressesUserQuery = getAddressesUserQuery;
            _getPhotoUserQuery = getPhotoUserQuery;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserResponse>> Post([FromBody] UserRequest request)
        {
            var result = await _createUserCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Add-Address")]
        [Authorize]
        public async Task<ActionResult<bool>> AddAddress([FromBody] UserAddressRequest request)
        {
            var result = await _addAddressUserCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Upload-Photo")]
        [Authorize]
        public async Task<ActionResult<UserPhotoResponse>> AddPhoto([FromForm] UserPhotoUploadRequest request)
        {
            if (request.File is null || request.File.Length == 0)
                return Problem("Nenhum arquivo foi enviado.");

            var allowdExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!allowdExtensions.Contains(extension))
                return Problem("O formato da imagem não é suportado!");

            using var stream = request.File.OpenReadStream();
            var photoRequest = new UserPhotoRequest(request.Id, request.Compress, request.Deletable);

            var result = await _addPhotoUserCommand.Execute(photoRequest, stream, extension);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _deleteUserCommand.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("Remove-Address/{userAddressId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveAddress(int userAddressId)
        {
            var result = await _deleteAddressUserCommand.Execute(userAddressId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("Remove-Photo/{userId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemovePhoto(int userId)
        {
            var result = await _deletePhotoUserCommand.Execute(userId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserResponse>> Put([FromBody] UserRequest request)
        {
            var result = await _updateUserCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<UserResponse>>> List()
        {
            var result = await _listUserQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> Get(int id)
        {
            var result = await _getUserQuery.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("Addresses/{id}")]
        [Authorize]
        public async Task<ActionResult<UserAddressesResponse>> GetAddresses(int id)
        {
            var result = await _getAddressesUserQuery.Execute(id);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("Get-Photos/{userId}")]
        [Authorize]
        public async Task<ActionResult<UserPhotoResponse>> GetPhotos(int userId)
        {
            var result = await _getPhotoUserQuery.Execute(userId);

            return ResultHandler.HandleResult(this, result);
        }
    }
}
