using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiscordCloneBackend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly IChannelService channelService;

        public ChannelsController(IChannelService channelService)
        {
            this.channelService = channelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChannels(int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var channels = await channelService.GetAllAsync(pageSize, pageNumber, "Messages,Profile,Server");
                return Ok(new ApiResponse(200, "Channels retrieved successfully.", channels));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching channels.", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChannelById(string id)
        {
            try
            {
                var channel = await channelService.GetChannelByIdAsync(id , 15, "Messages,Profile,Server");

                if (channel == null)
                {
                    return NotFound(new ApiResponse(404, "Channel not found."));
                }

                return Ok(new ApiResponse(200, "Channel retrieved successfully.", channel));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching the channel.", ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannelAsync([FromBody] ChannelRequestDTO channelRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(["Invalid input data."], 400));
            }

            try
            {
                var createdChannel = await channelService.CreateChannelAsync(channelRequestDTO);
                return CreatedAtAction(nameof(GetChannelById), new { id = createdChannel.Id },
                    new ApiResponse(201, "Channel created successfully.", createdChannel));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse(401, "You are not authorized to create a channel.", ex.Message));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("general"))
            {
                return BadRequest(new ApiResponse(400, "A channel named 'general' already exists in this server.", ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, "Failed to create channel.", ex.Message));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChannelAsync([FromBody] DeleteChannelRequestDto deleteChannelRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(["Invalid input data."], 400));
            }

            try
            {
                await channelService.DeleteChannelAsync(deleteChannelRequestDto);
                return Ok(new ApiResponse(200, "Channel deleted successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse(401, "You are not authorized to delete this channel.", ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, "Channel not found.", ex.Message));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("general"))
            {
                return BadRequest(new ApiResponse(400, "Cannot delete the 'general' channel.", ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, "Failed to delete channel.", ex.Message));
            }
        }

        [HttpPut]
        public async Task<IActionResult> ChangeChannelNameAsync([FromBody] ChangeChannelNameRequestDto changeChannelNameRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(["Invalid input data."], 400));
            }

            try
            {
                var updatedChannel = await channelService.ChangeChannelNameAsync(changeChannelNameRequestDto);
                return Ok(new ApiResponse(200, "Channel name updated successfully.", updatedChannel));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse(401, "You are not authorized to change the channel name.", ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, "Channel not found.", ex.Message));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("general"))
            {
                return BadRequest(new ApiResponse(400, "Cannot rename a channel to 'general'.", ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, "Failed to update channel name.", ex.Message));
            }
        }
    }
}
