using Simple.Ecommerce.App.Interfaces.Services.CredentialService;
using Simple.Ecommerce.Domain.Settings.EmailSettings;
using Simple.Ecommerce.Domain.Settings.SmtpSettings;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Simple.Ecommerce.Infra.Services.Email
{
    public class SmtpService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly EmailSettings _emailSettings;

        public SmtpService(
            SmtpSettings smtpSettings,
            EmailSettings emailSettings
        )
        {
            _smtpSettings = smtpSettings;
            _emailSettings = emailSettings;
        }

        public async Task SendEmailVerification(string to, string token)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "Confirme seu e-mail";

            var verificationLink = $"{_emailSettings.VerificationBaseUrl}?token={token}";
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <h2>Confirme seu e-amil</h2>
                    <p>Clique no link abaixo para validar seu e-mail:</p>
                    <a href='{verificationLink}'>Confirmar meu e-mail</a>"
            };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
