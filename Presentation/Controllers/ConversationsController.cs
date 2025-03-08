using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiscordCloneBackend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            this.conversationService = conversationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConversations([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            try
            {
                var conversations = await conversationService.GetAllAsync(pageSize, pageNumber, "MemberOne,MemberTwo,DirectMessages");

                var response = new ApiResponse(200, "Conversations retrieved successfully.", conversations);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(400, $"Invalid pagination parameters: {ex.Message}"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching conversations.", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversationById(string id)
        {
            try
            {
                var conversation = await conversationService.GetByIdAsync(id, "MemberOne,MemberTwo,DirectMessages");

                if (conversation == null)
                {
                    return NotFound(new ApiResponse(404, "Conversation not found."));
                }

                return Ok(new ApiResponse(200, "Conversation retrieved successfully.", conversation));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(400, $"Invalid conversation ID: {ex.Message}"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching the conversation.", ex.Message));
            }
        }

        [HttpGet("by-members")]
        public async Task<IActionResult> GetOrCreateConversationByMembers([FromQuery] string memberOneId, [FromQuery] string memberTwoId , [FromQuery] int directMessagesLimit)
        {
            try
            {
                var conversation = await conversationService.GetOrCreateConversationByMembersAsync(memberOneId, memberTwoId , directMessagesLimit = 15);
                return Ok(new ApiResponse(200, "Conversation retrieved or created successfully.", conversation));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(400, $"Invalid member IDs: {ex.Message}"));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse(409, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while fetching or creating the conversation.", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConversation(string id)
        {
            try
            {
                var conversation = await conversationService.GetByIdAsync(id, "MemberOne,MemberTwo,DirectMessages");
                if (conversation == null)
                {
                    return NotFound(new ApiResponse(404, "Conversation not found."));
                }

                await conversationService.DeleteAsync(id);
                return Ok(new ApiResponse(200, "Conversation deleted successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(400, $"Invalid conversation ID: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An error occurred while deleting the conversation.", ex.Message));
            }
        }
    }
}