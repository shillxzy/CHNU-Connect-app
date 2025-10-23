namespace CHNU_Connect.BLL.DTOs.Auth;

public class LoginRequestDto
{
    public string Email { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}