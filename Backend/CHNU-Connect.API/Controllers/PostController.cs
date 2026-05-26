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
        private readonly IActivityLogService _activityLogService;

        public PostController(IPostService postService, ILogger<PostController> logger, IActivityLogService activityLogService)
        {
            _postService = postService;
            _logger = logger;
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var posts = await _postService.GetAllAsync(currentUserId);

                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all posts");
                return StatusCode(500, new { message = "An error occurred while retrieving posts." });
            }
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var items = await _postService.GetFeedAsync(currentUserId, page, pageSize);
                var totalCount = await _postService.GetTotalCountAsync();

                return Ok(new { items, totalCount, page, pageSize });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feed");
                return StatusCode(500, new { message = "An error occurred while retrieving feed." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var post = await _postService.GetByIdAsync(id, currentUserId);

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
                var currentUserId = GetCurrentUserId();
                var posts = await _postService.GetByUserIdAsync(userId, currentUserId);

                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting posts for user: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving user posts." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "teacher,admin,superAdmin")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var post = await _postService.CreatePostAsync(request, currentUserId.Value);

                var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
                await _activityLogService.LogAsync(currentUserId.Value, userName, "post_created", "Post", post.Id);
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
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var post = await _postService.GetByIdAsync(id, currentUserId);

                if (post == null)
                    return NotFound(new { message = "Post not found." });

                var isAdmin = User.IsInRole("admin") || User.IsInRole("superAdmin");
                if (post.UserId != currentUserId.Value && !isAdmin)
                    return StatusCode(403, new { message = "You can only edit your own posts." });

                var updatedPost = await _postService.UpdatePostAsync(id, request, currentUserId);

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

                var post = await _postService.GetByIdAsync(id, currentUserId);

                if (post == null)
                    return NotFound(new { message = "Post not found." });

                var isAdmin = User.IsInRole("admin") || User.IsInRole("superAdmin");
                if (post.UserId != currentUserId.Value && !isAdmin)
                    return StatusCode(403, new { message = "You can only delete your own posts." });

                var success = await _postService.DeletePostAsync(id);

                if (!success)
                    return BadRequest(new { message = "Failed to delete post." });

                var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
                await _activityLogService.LogAsync(currentUserId.Value, userName, "post_deleted", "Post", id);
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchPosts([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest(new { message = "Search term is required." });

                var currentUserId = GetCurrentUserId();
                var posts = await _postService.SearchPostsAsync(searchTerm, currentUserId);

                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching posts with term: {SearchTerm}", searchTerm);
                return StatusCode(500, new { message = "An error occurred while searching posts." });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpPost("with-image")]
        [Authorize(Roles = "teacher,admin,superAdmin")]
        public async Task<IActionResult> CreatePostWithImage([FromForm] CreatePostWithImageDto request)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            string? imageUrl = null;

            if (request.Image != null && request.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }

                imageUrl = $"/uploads/{fileName}";
            }

            var postDto = new CreatePostDto
            {
                Content = request.Content,
                ImageUrl = imageUrl
            };

            var post = await _postService.CreatePostAsync(postDto, currentUserId.Value);

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        public class CreatePostWithImageDto
        {
            public string Content { get; set; } = "";
            public IFormFile? Image { get; set; }
        }

        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdatePostImage(int id, IFormFile image)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var post = await _postService.GetByIdAsync(id, currentUserId);
            if (post == null) return NotFound(new { message = "Post not found." });

            var isAdmin = User.IsInRole("admin") || User.IsInRole("superAdmin");
            if (post.UserId != currentUserId.Value && !isAdmin)
                return StatusCode(403, new { message = "You can only edit your own posts." });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/{fileName}";
            await _postService.UpdatePostAsync(id, new UpdatePostDto { Content = post.Content ?? "", ImageUrl = imageUrl }, currentUserId);

            return Ok(new { imageUrl });
        }
    }
}
