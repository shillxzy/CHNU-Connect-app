using CHNU_Connect.BLL.DTOs.Comment;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            try
            {
                var comment = await _commentService.GetByIdAsync(id);
                if (comment == null)
                    return NotFound(new { message = "Comment not found." });

                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment: {CommentId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the comment." });
            }
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsByPost(int postId)
        {
            try
            {
                var comments = await _commentService.GetByPostIdAsync(postId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for post: {PostId}", postId);
                return StatusCode(500, new { message = "An error occurred while retrieving post comments." });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCommentsByUser(int userId)
        {
            try
            {
                var comments = await _commentService.GetByUserIdAsync(userId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for user: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving user comments." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                request.UserId = currentUserId.Value;
                var comment = await _commentService.CreateCommentAsync(request);
                
                _logger.LogInformation("Comment created by user: {UserId} on post: {PostId}", currentUserId, request.PostId);
                return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while creating the comment." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CreateCommentDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var comment = await _commentService.GetByIdAsync(id);
                if (comment == null)
                    return NotFound(new { message = "Comment not found." });

                // Check if user owns the comment
                if (comment.UserId != currentUserId.Value)
                    return Forbid("You can only edit your own comments.");

                var updatedComment = await _commentService.UpdateCommentAsync(id, request);
                
                _logger.LogInformation("Comment updated: {CommentId} by user: {UserId}", id, currentUserId);
                return Ok(updatedComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment: {CommentId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the comment." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var comment = await _commentService.GetByIdAsync(id);
                if (comment == null)
                    return NotFound(new { message = "Comment not found." });

                // Check if user owns the comment
                if (comment.UserId != currentUserId.Value)
                    return Forbid("You can only delete your own comments.");

                var success = await _commentService.DeleteCommentAsync(id);
                if (!success)
                    return BadRequest(new { message = "Failed to delete comment." });

                _logger.LogInformation("Comment deleted: {CommentId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Comment deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment: {CommentId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the comment." });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
