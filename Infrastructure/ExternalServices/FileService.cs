using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DiscordCloneBackend.Core.Interfaces.IExternalServices;
using dotenv.net;

namespace DiscordCloneBackend.Infrastructure.ExternalServices
{
    public class FileService : IFileService
    {
        private readonly Cloudinary cloudinary;
        public FileService(IConfiguration configuration)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));

            var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
            cloudinary = new Cloudinary(cloudinaryUrl);
            cloudinary.Api.Secure = true;
        }
        public async void DeleteFile(string filePath, string folderName)
        {
            try
            {
                var publicId = $"{folderName}/{filePath}";

                var deleteParams = new DeletionParams(publicId);
                var deleteResult = await cloudinary.DestroyAsync(deleteParams);

                if (deleteResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("File deleted successfully.");
                }
                else
                {
                    throw new InvalidOperationException($"Failed to delete file: {deleteResult.Error?.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error deleting file from Cloudinary", ex);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string uploadFolder)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is required.");
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = uploadFolder
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var fileUrl = uploadResult.SecureUrl.ToString();

                if (file.FileName.EndsWith(".pdf") || file.FileName.EndsWith(".docx") || file.FileName.EndsWith(".txt"))
                {
                    return $"{fileUrl}?attachment=true";
                }

                return fileUrl;
            }
            else
            {
                throw new Exception("Failed to upload file to Cloudinary.");
            }
        }

    }
}
