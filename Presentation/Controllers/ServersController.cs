using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DiscordCloneBackend.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DiscordCloneBackend.Application.Services;

namespace DiscordCloneBackend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly IServerService serverService;

        public ServersController(IServerService serverService)
        {
            this.serverService = serverService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServers(int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var servers = await serverService.GetAllAsync(pageSize, pageNumber, "Profile,Members,Channels");
                return Ok(new ApiResponse(200, "Servers fetched successfully", servers));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServerById(string id)
        {
            try
            {
                var server = await serverService.GetByIdAsync(id , "Profile,Members,Channels");
                return Ok(new ApiResponse(200, "Server found", server));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

       /* [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetServersProfileId(string profileId)
        {
            try
            {
                if (string.IsNullOrEmpty(profileId))
                {
                    return BadRequest("profile id must be added");
                }

                var servers = await serverService.GetServersByProfileIdAsync(profileId);
                return Ok(new ApiResponse(200, "Servers fetched successfully for profile", servers));
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(400, new ApiResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }*/

        [HttpGet("member/profile/{profileId}")]
        public async Task<IActionResult> GetServersBymemberProfileId(string profileId)
        {
            try
            {
                if(string.IsNullOrEmpty(profileId))
                {
                    return BadRequest("profile id must be added");
                }

                var servers = await serverService.GetServersByProfileIdAsync(profileId);
                return Ok(new ApiResponse(200, "Servers fetched successfully for profile", servers));
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(400 , new ApiResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("invite/{inviteCode}")]
        public async Task<IActionResult> GetServerByInviteCode(string inviteCode)
        {
            try
            {
                var server = await serverService.GetServerByInviteCodeAsync(inviteCode);
                return Ok(new ApiResponse(200, "Server found by invite code", server));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("profile/{profileId}/invite/{inviteCode}")]
        public async Task<IActionResult> GetServerByInviteCodeAndProfileId(string inviteCode, string profileId)
        {
            try
            {
                var server = await serverService.GetServerByInviteCodeAndProfileIdAsync(inviteCode, profileId);

                if (server == null)
                {
                    return NotFound(new ApiResponse(404, "Server not found with the given invite code."));
                }

                return Ok(new ApiResponse(200, "Server found, and you are a member", server ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse(403, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, "An unexpected error occurred", ex.Message));
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateServer([FromForm] ServerRequestDTO serverRequest)
        {

            try
            {
                var server = await serverService.AddServerWithImageAsync(serverRequest);
                return StatusCode(StatusCodes.Status201Created, new ApiResponse(201, "Server created successfully" , server));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServer(string id, [FromForm] ServerRequestDTO serverRequest)
        {
            try
            {
                var updatedServer = await serverService.UpdateServerWithImageAsync(id, serverRequest);

                if (updatedServer == null)
                {
                    return NotFound(new ApiResponse(404, $"Server with ID {id} not found."));
                }

                return Ok(new ApiResponse(200, "Server updated successfully", updatedServer));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, "Failed to update the server. " + ex.Message));
            }
        }

        [HttpDelete("{id}/profile/{profileId}")]
        public async Task<IActionResult> DeleteServer(string id, string profileId)
        {
            try
            {
                await serverService.DeleteServerAsync(id, profileId);
                return Ok(new ApiResponse(200, "Server deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(403, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, "An unexpected error occurred while deleting the server."));
            }
        }

        [HttpPost("{serverId}/generate-invite-code")]
        public async Task<IActionResult> GenerateNewInviteCode(string serverId)
        {
            try
            {
                var inviteCode = await serverService.GenerateNewInviteCodeAsync(serverId);
                return Ok(new ApiResponse(200, "New invite code generated successfully" , inviteCode));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("details/{serverId}")]
        public async Task<IActionResult> GetServer(string serverId)
        {
            try
            {
                var serverDto = await serverService.GetServerByIdWithDetailsAsync(serverId);
                return Ok( new ApiResponse(200 , "server retrived successfully" ,  serverDto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
