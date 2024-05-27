namespace Application.Common.Interfaces.Entities.Users.DTOs;

public record UserDataResponse(Guid Id, string Email, string Username, string FullName);