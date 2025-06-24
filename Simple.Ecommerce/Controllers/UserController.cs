using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Commands.UserCommands;
using Simple.Ecommerce.App.Interfaces.Queries.UserQueries;
using Simple.Ecommerce.Contracts.UserAddressContracts;
using Simple.Ecommerce.Contracts.UserCardContracts;
using Simple.Ecommerce.Contracts.UserContracts;
using Simple.Ecommerce.Contracts.UserPhotoContracts;

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
        private readonly IAddCardUserCommand _addCardUserCommand;
        private readonly IAddPhotoUserCommand _addPhotoUserCommand;
        private readonly IRemoveAddressUserCommand _deleteAddressUserCommand;
        private readonly IRemoveCardUserCommand _deleteCardUserCommand;
        private readonly IRemovePhotoUserCommand _deletePhotoUserCommand;
        private readonly IGetAddressesUserQuery _getAddressesUserQuery;
        private readonly IGetCardsUserQuery _getCardsUserQuery;
        private readonly IGetPhotoUserQuery _getPhotoUserQuery;

        public UserController(
            ICreateUserCommand createUserCommand, 
            IDeleteUserCommand deleteUserCommand, 
            IUpdateUserCommand updateUserCommand,
            IGetUserQuery getUserQuery,
            IListUserQuery listUserQuery,
            IAddAddressUserCommand addAddressUserCommand, 
            IAddCardUserCommand addCardUserCommand,
            IAddPhotoUserCommand addPhotoUserCommand,
            IRemoveAddressUserCommand deleteAddressUserCommand,
            IRemoveCardUserCommand deleteCardUserCommand,
            IRemovePhotoUserCommand deletePhotoUserCommand,
            IGetAddressesUserQuery getAddressesUserQuery, 
            IGetCardsUserQuery getCardsUserQuery,
            IGetPhotoUserQuery getPhotoUserQuery
        )
        {
            _createUserCommand = createUserCommand;
            _deleteUserCommand = deleteUserCommand;
            _updateUserCommand = updateUserCommand;
            _getUserQuery = getUserQuery;
            _listUserQuery = listUserQuery;
            _addAddressUserCommand = addAddressUserCommand;
            _addCardUserCommand = addCardUserCommand;
            _addPhotoUserCommand = addPhotoUserCommand;
            _deleteAddressUserCommand = deleteAddressUserCommand;
            _deleteCardUserCommand = deleteCardUserCommand;
            _deletePhotoUserCommand = deletePhotoUserCommand;
            _getAddressesUserQuery = getAddressesUserQuery;
            _getCardsUserQuery = getCardsUserQuery;
            _getPhotoUserQuery = getPhotoUserQuery;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserResponse>> Post([FromBody] UserRequest request)
        {
            var result = await _createUserCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Address")]
        [Authorize]
        public async Task<ActionResult<bool>> AddAddress([FromBody] UserAddressRequest request)
        {
            var result = await _addAddressUserCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Card")]
        [Authorize]
        public async Task<ActionResult<bool>> AddCard([FromBody] UserCardRequest request)
        {
            var result = await _addCardUserCommand.Execute(request);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpPost("Photo")]
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

        [HttpDelete("Address/{userAddressId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveAddress(int userAddressId)
        {
            var result = await _deleteAddressUserCommand.Execute(userAddressId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("Card/{userCardId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveCard(int userCardId)
        {
            var result = await _deleteCardUserCommand.Execute(userCardId);
         
            return ResultHandler.HandleResult(this, result);
        }

        [HttpDelete("Photo/{userId}")]
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

        [HttpGet("Address/{userId}")]
        [Authorize]
        public async Task<ActionResult<UserAddressesResponse>> GetAddresses(int userId)
        {
            var result = await _getAddressesUserQuery.Execute(userId);

            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("Card/{userId}")]
        [Authorize]
        public async Task<ActionResult<UserCardsReponse>> GetCards(int userId)
        {
            var result = await _getCardsUserQuery.Execute(userId);
         
            return ResultHandler.HandleResult(this, result);
        }

        [HttpGet("Photo/{userId}")]
        [Authorize]
        public async Task<ActionResult<UserPhotoResponse>> GetPhotos(int userId)
        {
            var result = await _getPhotoUserQuery.Execute(userId);

            return ResultHandler.HandleResult(this, result);
        }
    }
}
