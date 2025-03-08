namespace DiscordCloneBackend.Core.Interfaces.IExternalServices
{
    public interface IEmailService
    {
        Task SendMailAsync(string toEmail, string subject, string templateName, Dictionary<string, string> placeholders);
    }
}
