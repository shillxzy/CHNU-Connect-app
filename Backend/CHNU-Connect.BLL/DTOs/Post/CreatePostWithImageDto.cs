using Microsoft.AspNetCore.Http;


namespace CHNU_Connect.BLL.DTOs.Post
{
    public class CreatePostWithImageDto
    {
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
    }
}
