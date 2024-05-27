namespace Application.Common.Interfaces.Entities.Users.DTOs;

public record UserResponse(Guid Id, string Email, string FullName, string Username, string AccessToken);