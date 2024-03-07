namespace Nitroterm.Backend.Dto;

public record LoginResponseDto(UserDto User, string Token);