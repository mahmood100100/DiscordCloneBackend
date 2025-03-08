using AutoMapper;
using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Application.Services;
using DiscordCloneBackend.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordCloneBackend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService memberService;

        public MembersController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMembers(int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var members = await memberService.GetAllAsync(pageSize, pageNumber,
                    "Messages,ConversationsInitiated,ConversationsReceived,DirectMessages,Profile,Server");
                return Ok(new ApiResponse(200, "Members fetched successfully", members));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(string id)
        {
            try
            {
                var member = await memberService.GetByIdAsync(id, 
                    "Messages,ConversationsInitiated,ConversationsReceived,DirectMessages,Profile,Server");
                return Ok(new ApiResponse(200, "Member found", member));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMember([FromBody] MemberRequestDTO memberRequest)
        {
            try
            {
                var member = await memberService.AddMemberToServerAsync(memberRequest, MemberRole.Guest);
                return Ok(new ApiResponse(201, "Member added successfully", member));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse(409, ex.Message));
            }
            catch (AutoMapperMappingException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, "Failed to map member data.", ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, "An unexpected error occurred", ex.Message));
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(string id, [FromBody] MemberRequestDTO memberRequest)
        {
            try
            {
                await memberService.UpdateAsync(id, memberRequest);
                return Ok(new ApiResponse(200, "Member updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMember([FromBody] DeleteMemberRequestDto deleteMemberRequestDto)
        {
            try
            {
                await memberService.DeleteMemberAsync(deleteMemberRequestDto);

                return Ok(new ApiResponse(200, "Member deleted successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, "An unexpected error occurred: " + ex.Message));
            }
        }

        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeMemberRoleAsync([FromBody] ChangeMemberRoleRequestDto changeMemberRoleRequestDto)
        {
            try
            {
                var updatedMember = await memberService.ChangeMemberRoleAsync(changeMemberRoleRequestDto);
                return Ok(new ApiResponse(200 , "Role Changed Successfully" , updatedMember));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("soft-delete")]
        public async Task<IActionResult> SoftDeleteMember([FromBody] DeleteMemberRequestDto deleteMemberRequestDto)
        {
            try
            {
                if (deleteMemberRequestDto == null ||
                    string.IsNullOrEmpty(deleteMemberRequestDto.RequesterProfileId) ||
                    string.IsNullOrEmpty(deleteMemberRequestDto.ServerId) ||
                    string.IsNullOrEmpty(deleteMemberRequestDto.TargetMemberId))
                {
                    return BadRequest("Invalid request data. All fields (RequesterProfileId, ServerId, TargetMemberId) are required.");
                }

                await memberService.SoftDeleteMemberAsync(deleteMemberRequestDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while soft-deleting the member.");
            }
        }


        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<MemberResponseDTO>>> GetByRole(MemberRole role)
        {
            try
            {
                var members = await memberService.GetMembersByRoleAsync(role);
                return Ok(new ApiResponse(200, "members retireived successfully", members));

            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
            
        }

        [HttpGet("server/{serverId}")]
        public async Task<ActionResult<IEnumerable<MemberResponseDTO>>> GetByServerId(string serverId)
        {
            try
            {
                var members = await memberService.GetMembersByServerIdAsync(serverId);
                return Ok(new ApiResponse(200 , "members retireived successfully" , members));

            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
            
        }

    }
}
