namespace KnowledgeCheck.BLL.DTOs.Auth;

public class RefreshTokenResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}