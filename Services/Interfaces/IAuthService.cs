using EduSync.API.DTOs;
using EduSync.API.Models;

namespace EduSync.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponseDto> GetUserByIdAsync(int userId);
        string GenerateJwtToken(User user);
    }
} 