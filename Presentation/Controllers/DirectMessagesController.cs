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
    public class DirectMessagesController : ControllerBase
    {
        private readonly IDirectMessageService directMessageService;

        public DirectMessagesController(IDirectMessageService directMessageService)
        {
            this.directMessageService = directMessageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDirectMessages(int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var messages = await directMessageService.GetAllAsync(pageSize, pageNumber, "Member,Member.Profile,Conversation");
                return Ok(new ApiResponse(200, "Direct messages retrieved successfully.", messages));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching direct messages.", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDirectMessageById(string id)
        {
            try
            {
                var message = await directMessageService.GetByIdAsync(id, "Member,Member.Profile,Conversation");

                if (message == null)
                {
                    return NotFound(new ApiResponse(404, "Direct message not found."));
                }

                return Ok(new ApiResponse(200, "Direct message retrieved successfully.", message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching the direct message.", ex.Message));
            }
        }

        [HttpGet("by-conversation/{conversationId}")]
        public async Task<IActionResult> GetDirectMessagesByConversationId(string conversationId)
        {
            try
            {
                var messages = await directMessageService.GetDirectMessagesByConversationIdAsync(conversationId , pageSize:15);
                return Ok(new ApiResponse(200, "Direct messages retrieved successfully.", messages));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching messages.", ex.Message));
            }
        }

        [HttpGet("by-member/{memberId}")]
        public async Task<IActionResult> GetDirectMessagesByMemberId(string memberId)
        {
            try
            {
                var messages = await directMessageService.GetDirectMessagesByMemberIdAsync(memberId);
                return Ok(new ApiResponse(200, "Direct messages retrieved successfully.", messages));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching messages.", ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDirectMessage([FromForm] DirectMessageRequestDTO requestDTO)
        {
            try
            {
                var directMessage = await directMessageService.CreateMessageAsync(requestDTO);
                return Ok(new ApiResponse(201, "Direct message sent successfully." , directMessage));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while sending the message.", ex.Message));
            }
        }

        [HttpDelete("{messageId}/soft")]
        public async Task<IActionResult> SoftDeleteDirectMessage(string messageId)
        {
            try
            {
                await directMessageService.SoftDeleteAsync(messageId);

                return Ok(new ApiResponse(200, "Direct message soft deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while soft deleting the direct message.", ex.Message));
            }
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> HardDeleteDirectMessage(string id)
        {
            try
            {
                await directMessageService.HardDeleteAsync(id);
                
                return Ok(new ApiResponse(200, "Direct message hard deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while hard deleting the direct message.", ex.Message));
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateDirectMessage([FromForm] ChangeMessageRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedMessage = await directMessageService.UpdateDirectMessageAsync(request);
                return Ok(new ApiResponse(200 , "Direct Message Updated Successfully" , updatedMessage));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }

    }
}
