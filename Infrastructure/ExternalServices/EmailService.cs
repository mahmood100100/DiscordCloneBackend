using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using DiscordCloneBackend.Core.Interfaces.IExternalServices;

namespace DiscordCloneBackend.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendMailAsync(string toEmail, string subject, string templateName, Dictionary<string, string> placeholders)
        {
            var emailMessage = new MimeMessage();
            string senderEmail = configuration["EmailSettings:SenderEmail"];
            string password = configuration["EmailSettings:Password"];

            // Log email and password for debugging (REMOVE in production)
            Console.WriteLine($"[DEBUG] Sender Email: {senderEmail}");
            Console.WriteLine($"[DEBUG] Password: {password}"); // ⚠️ Do NOT log passwords in production!

            emailMessage.From.Add(new MailboxAddress("mahmoudEmail", senderEmail));
            emailMessage.To.Add(new MailboxAddress("client", toEmail));
            emailMessage.Subject = subject;

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "EmailTemplates", templateName);
            string emailBody = await File.ReadAllTextAsync(templatePath);

            foreach (var placeholder in placeholders)
            {
                emailBody = emailBody.Replace(placeholder.Key, placeholder.Value);
            }

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailBody };

            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(configuration["EmailSettings:SmtpServer"],
                int.Parse(configuration["EmailSettings:Port"]),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(senderEmail, password);

            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }

    }
}
