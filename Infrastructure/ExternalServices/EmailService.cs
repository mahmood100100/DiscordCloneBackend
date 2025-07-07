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
            Console.WriteLine("[DEBUG] Starting email sending process...");

            string senderEmail = configuration["EmailSettings:SenderEmail"];
            string password = configuration["EmailSettings:Password"];
            string smtpServer = configuration["EmailSettings:SmtpServer"];
            string smtpPortStr = configuration["EmailSettings:Port"];

            Console.WriteLine($"[DEBUG] SenderEmail: {senderEmail}");
            Console.WriteLine($"[DEBUG] SMTP Server: {smtpServer}");
            Console.WriteLine($"[DEBUG] Port: {smtpPortStr}");

            if (string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(smtpServer) ||
                string.IsNullOrWhiteSpace(smtpPortStr))
            {
                Console.WriteLine("[ERROR] One or more email configuration values are missing.");
                return;
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Discord Clone", senderEmail));
            emailMessage.To.Add(new MailboxAddress("Recipient", toEmail));
            emailMessage.Subject = subject;

            try
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "EmailTemplates", templateName);
                Console.WriteLine($"[DEBUG] Loading email template from: {templatePath}");

                string emailBody = await File.ReadAllTextAsync(templatePath);

                foreach (var placeholder in placeholders)
                {
                    emailBody = emailBody.Replace(placeholder.Key, placeholder.Value);
                }

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailBody };
                Console.WriteLine("[DEBUG] Email body prepared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load or process email template: {ex.Message}");
                return;
            }

            try
            {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                Console.WriteLine("[DEBUG] Connecting to SMTP server...");
                await client.ConnectAsync(smtpServer, int.Parse(smtpPortStr), SecureSocketOptions.StartTls);
                Console.WriteLine("[DEBUG] Connected.");

                Console.WriteLine("[DEBUG] Authenticating...");
                await client.AuthenticateAsync(senderEmail, password);
                Console.WriteLine("[DEBUG] Authenticated.");

                Console.WriteLine("[DEBUG] Sending email...");
                await client.SendAsync(emailMessage);
                Console.WriteLine($"[DEBUG] Email send command finished at {DateTime.UtcNow:O}");
                Console.WriteLine("[DEBUG] Email sent successfully.");

                await client.DisconnectAsync(true);
                Console.WriteLine("[DEBUG] Disconnected from SMTP server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed during SMTP operation: {ex.Message}");
            }
        }
    }
}
