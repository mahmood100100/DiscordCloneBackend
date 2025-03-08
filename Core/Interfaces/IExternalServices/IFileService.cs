namespace DiscordCloneBackend.Core.Interfaces.IExternalServices
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string uploadPath);
        void DeleteFile(string filePath, string folderName);
    }
}
