using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiscordCloneBackend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService profileService;

        public ProfilesController(IProfileService profileService)
        {
            this.profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProfiles(int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var profiles = await profileService.GetAllAsync(pageSize, pageNumber , "Servers,Members,Channels");
                return Ok(new ApiResponse(200, "Profiles fetched successfully", profiles));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileById(string id)
        {
            try
            {
                var profile = await profileService.GetByIdAsync(id , "Servers,Members,Channels");
                return Ok(new ApiResponse(200, "Profile found", profile));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetProfileByEmail(string email)
        {
            try
            {
                var profile = await profileService.GetProfileByEmailAsync(email);
                return Ok(new ApiResponse(200, "Profile found", profile));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetProfilesByRole(string role)
        {
            try
            {
                var profiles = await profileService.GetProfilesByRoleAsync(role);
                return Ok(new ApiResponse(200, "Profiles found", profiles));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }
    }
}
