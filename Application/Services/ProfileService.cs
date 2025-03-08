using AutoMapper;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using DiscordCloneBackend.Core.Interfaces.IExternalServices;
using DiscordCloneBackend.Core.Interfaces.IRepositories;

namespace DiscordCloneBackend.Application.Services
{
    public class ProfileService : GenericService<DiscordCloneBackend.Core.Entities.Profile, ProfileRequestDTO, ProfileResponseDTO>, IProfileService
    {
        private readonly IFileService fileService;

        public ProfileService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileService fileService)
            : base(mapper, unitOfWork)
        {
            this.fileService = fileService;
        }

        public async Task CreateProfileAsync(UserRequestDTO request , string userId)
        {
            var imageUrl = await fileService.UploadFileAsync(request.Image, "ProfileImages");

            var profile = new DiscordCloneBackend.Core.Entities.Profile
            {
                UserId = userId,
                Name = request.Name,
                Email = request.Email,
                ImageUrl = imageUrl,
                UserName = request.UserName
            };

            await unitOfWork.Profiles.AddAsync(profile);
            await unitOfWork.CompleteAsync();
        }

        public async Task DeleteProfileByUserIdAsync(string userId)
        {
            var profile = await unitOfWork.Profiles.GetProfileByUserIdAsync(userId);
            if (profile == null)
            {
                throw new KeyNotFoundException($"Profile with userID {userId} not found.");
            }

            if (!string.IsNullOrEmpty(profile.ImageUrl))
            {
                var fileName = Path.GetFileNameWithoutExtension(profile.ImageUrl.Split('/').Last().Split('?').First());
                fileService.DeleteFile(fileName, "ProfileImages");
            }


            await unitOfWork.Profiles.DeleteProfileAsync(userId);
            Console.WriteLine(profile.Id);

            await unitOfWork.CompleteAsync();
        }

        public async Task<ProfileResponseDTO> GetProfileByEmailAsync(string email)
        {
            var profile = await unitOfWork.Profiles.GetProfileByEmailAsync(email);
            return mapper.Map<ProfileResponseDTO>(profile);
        }

        public async Task<IEnumerable<ProfileResponseDTO>> GetProfilesByRoleAsync(string role)
        {
            var profiles = await unitOfWork.Profiles.GetProfilesByRoleAsync(role);
            return mapper.Map<IEnumerable<ProfileResponseDTO>>(profiles);
        }

        public async Task<ProfileResponseDTO> GetProfileByUserIdAsync(string id)
        {
            var profile = await unitOfWork.Profiles.GetProfileByUserIdAsync(id);
            return mapper.Map<ProfileResponseDTO>(profile);
        }
    }
}
