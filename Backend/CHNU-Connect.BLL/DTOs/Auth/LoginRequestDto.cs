using System.ComponentModel.DataAnnotations;

namespace CHNU_Connect.BLL.DTOs.Auth;

public class LoginRequestDto
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}