namespace ShoppingApp.Application.DTOs.Auth;

public record AuthResponseDto(string Token, DateTime Expiration, string Email, string Role);
