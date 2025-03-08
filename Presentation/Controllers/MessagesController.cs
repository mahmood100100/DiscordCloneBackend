using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiscordCloneBackend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService messageService;

        public MessagesController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessages(int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var messages = await messageService.GetAllAsync(pageSize, pageNumber, "Member,Member.Profile,Channel");
                return Ok(new ApiResponse(200, "Messages retrieved successfully.", messages));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching ,messages.", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessageById(string id)
        {
            try
            {
                var message = await messageService.GetByIdAsync(id, "Member,Member.Profile,Channel");

                if (message == null)
                {
                    return NotFound(new ApiResponse(404, "Message not found."));
                }

                return Ok(new ApiResponse(200, "Message retrieved successfully.", message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching the message.", ex.Message));
            }
        }

        [HttpGet("channel/{channelId}")]
        public async Task<IActionResult> GetMessagesByChannelId(
          string channelId,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 10)
        {
            try
            {
                var messages = await messageService.GetMessagesByChannelIdAsync(channelId, page, pageSize);

                if (!messages.Any())
                    return NotFound(new ApiResponse(404, "No messages found in this channel."));

                return Ok(new ApiResponse(200, "Messages retrieved successfully.", new
                {
                    Messages = messages,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = await messageService.GetMessagesCountByChannelIdAsync(channelId)
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching messages.", ex.Message));
            }
        }

        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetMessagesByMemberId(string memberId)
        {
            try
            {
                var messages = await messageService.GetMessagesByMemberIdAsync(memberId);

                if (!messages.Any())
                    return NotFound(new ApiResponse(404, "No messages found for this member."));

                return Ok(new ApiResponse(200, "Messages retrieved successfully.", messages));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching messages.", ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessageAsync([FromForm] MessageRequestDTO messageRequestDTO)
        {
            try
            {
                var createdMessage = await messageService.CreateMessageAsync(messageRequestDTO);
                return CreatedAtAction(nameof(GetMessageById), new { id = createdMessage.Id },
                    new ApiResponse(201, "Message created successfully.", createdMessage));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, "Failed to create message.", ex.Message));
            }
        }

        [HttpDelete("{id}/soft")]
        public async Task<IActionResult> SoftDeleteMessageAsync(string id)
        {
            try
            {
                await messageService.SoftDeleteAsync(id);
                return Ok(new ApiResponse(200, "Message marked as deleted successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, "Message not found.", ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, "Failed to mark message as deleted.", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDeleteMessageAsync(string id)
        {
            try
            {
                await messageService.HardDeleteAsync(id);
                return Ok(new ApiResponse(200, "Message deleted permanently."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, "Message not found.", ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, "Failed to delete message.", ex.Message));
            }
        }

        [HttpPut]
        public async Task<ActionResult<MessageResponseDTO>> UpdateMessage([FromForm] ChangeMessageRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var updatedMessage = await messageService.UpdateMessageAsync(request);
                return Ok(new ApiResponse(200 , "message updated successfully" , updatedMessage));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while updating the message.");
            }
        }
    }
}
