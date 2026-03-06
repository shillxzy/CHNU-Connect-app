using System.ComponentModel.DataAnnotations;

namespace CHNU_Connect.BLL.DTOs.Auth;

public class RefreshTokenRequestDto
{
    [Required]
    public string Token { get; set; }
}