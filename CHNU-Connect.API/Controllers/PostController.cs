using CHNU_Connect.BLL.DTOs.Post;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _postService.GetAllAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all posts");
                return StatusCode(500, new { message = "An error occurred while retrieving posts." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            try
            {
                var post = await _postService.GetByIdAsync(id);
                if (post == null)
                    return NotFound(new { message = "Post not found." });

                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting post: {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the post." });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPostsByUser(int userId)
        {
            try
            {
                var posts = await _postService.GetByUserIdAsync(userId);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting posts for user: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving user posts." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto request, int? currentUserId)
        {
            try
            {
                currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                request.AuthorId = currentUserId.Value;
                var post = await _postService.CreatePostAsync(request);
                
                _logger.LogInformation("Post created by user: {UserId}", currentUserId);
                return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while creating the post." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] CreatePostDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var post = await _postService.GetByIdAsync(id);
                if (post == null)
                    return NotFound(new { message = "Post not found." });

                // Check if user owns the post
                if (post.AuthorId != currentUserId.Value)
                    return Forbid("You can only edit your own posts.");

                var updatedPost = await _postService.UpdatePostAsync(id, request);
                
                _logger.LogInformation("Post updated: {PostId} by user: {UserId}", id, currentUserId);
                return Ok(updatedPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post: {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the post." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var post = await _postService.GetByIdAsync(id);
                if (post == null)
                    return NotFound(new { message = "Post not found." });

                // Check if user owns the post
                if (post.AuthorId != currentUserId.Value)
                    return Forbid("You can only delete your own posts.");

                var success = await _postService.DeletePostAsync(id);
                if (!success)
                    return BadRequest(new { message = "Failed to delete post." });

                _logger.LogInformation("Post deleted: {PostId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Post deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post: {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the post." });
            }
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikePost(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var success = await _postService.LikePostAsync(id, currentUserId.Value);
                if (!success)
                    return BadRequest(new { message = "Post already liked or post not found." });

                _logger.LogInformation("Post liked: {PostId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Post liked successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking post: {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while liking the post." });
            }
        }

        [HttpDelete("{id}/like")]
        public async Task<IActionResult> UnlikePost(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var success = await _postService.UnlikePostAsync(id, currentUserId.Value);
                if (!success)
                    return BadRequest(new { message = "Post not liked or post not found." });

                _logger.LogInformation("Post unliked: {PostId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Post unliked successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unliking post: {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while unliking the post." });
            }
        }

        [HttpGet("{id}/likes")]
        public async Task<IActionResult> GetPostLikes(int id)
        {
            try
            {
                var likeCount = await _postService.GetLikeCountAsync(id);
                return Ok(new { postId = id, likeCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting likes for post: {PostId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving post likes." });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
