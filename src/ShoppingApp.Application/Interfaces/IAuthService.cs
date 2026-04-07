using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Auth;

namespace ShoppingApp.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto);
}
