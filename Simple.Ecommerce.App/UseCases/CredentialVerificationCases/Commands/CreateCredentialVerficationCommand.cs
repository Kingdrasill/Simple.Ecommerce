using Simple.Ecommerce.App.Interfaces.Commands.CredentialVerificationCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.CredentialService;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Contracts.CredentialVerificationContracts;
using Simple.Ecommerce.Domain.Entities.CredentialVerificationEntity;
using Simple.Ecommerce.Domain.Enums.Crendetial;
using Simple.Ecommerce.Domain.Objects;

namespace Simple.Ecommerce.App.UseCases.CredentialVerificationCases.Commands
{
    public class CreateCredentialVerficationCommand : ICreateCredentialVerificationCommand
    {
        private readonly ICredentialVerificationRepository _repository;
        private readonly ILoginRepository _loginRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly IEmailService _emailService;

        public CreateCredentialVerficationCommand(
            ICredentialVerificationRepository repository, 
            ILoginRepository loginRepository,
            ISaverTransectioner unityOfWork,
            IEmailService emailService
        )
        {
            _repository = repository;
            _loginRepository = loginRepository;
            _saverOrTransectioner = unityOfWork;
            _emailService = emailService;
        }

        public async Task<Result<CredentialVerificationResponse>> Execute(int loginId)
        {
            var getLogin = await _loginRepository.Get(loginId);
            if (getLogin.IsFailure)
            {
                return Result<CredentialVerificationResponse>.Failure(getLogin.Errors!);
            }

            var login = getLogin.GetValue();

            var instance = new CredentialVerification().Create(
                0,
                login.Id,
                Guid.NewGuid().ToString("N"),
                DateTime.UtcNow.AddHours(24)
            );
            if (instance.IsFailure) 
            {
                return Result<CredentialVerificationResponse>.Failure(instance.Errors!);
            }

            var createResult = await _repository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<CredentialVerificationResponse>.Failure(createResult.Errors!);
            }

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {                
                return Result<CredentialVerificationResponse>.Failure(commit.Errors!);
            }

            switch (login.Type)
            {
                case CredentialType.Email:
                    await _emailService.SendEmailVerification(getLogin.GetValue().Credential, createResult.GetValue().Token);
                    break;
                case CredentialType.Phone:
                    // Adicionar envio de verificação por sms
                    break;
                case CredentialType.Social:
                    // Adicionar verificação por rede social
                    break;
            }

            return Result<CredentialVerificationResponse>.Success(
                new CredentialVerificationResponse(createResult.GetValue().Token)
            );
        }
    }
}
