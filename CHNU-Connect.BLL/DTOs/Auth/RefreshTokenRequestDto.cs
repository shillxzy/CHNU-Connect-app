namespace CHNU_Connect.BLL.DTOs.Auth;

public class RefreshTokenRequestDto
{
    public string RefreshToken  { get; set; } = null!;
    public string? IpAddress { get; set; }
}