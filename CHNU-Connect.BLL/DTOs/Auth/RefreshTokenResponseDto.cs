namespace CHNU_Connect.BLL.DTOs.Auth;

public class RefreshTokenResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Token { get; set; } = null!;
}