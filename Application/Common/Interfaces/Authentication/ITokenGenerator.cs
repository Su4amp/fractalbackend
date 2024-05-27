using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Authentication;

public interface ITokenGenerator
{
    TokensResponse GenerateTokens(Guid userId, string fullName);
}