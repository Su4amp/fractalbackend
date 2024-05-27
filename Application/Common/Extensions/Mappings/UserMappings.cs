using Application.Common.Interfaces.Entities.Users.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mappings;

public static class UserMappings
{
    public static UserDataResponse ToUserDataResponse(this User user)
    {
        return new UserDataResponse(
            Id: user.Id,
            FullName: user.FullName,
            Username: user.UserName!,
            Email: user.Email!);
    }

    public static UserResponse ToUserResponse(this User user, TokensResponse tokens)
    {
        return new UserResponse(
            Id: user.Id,
            FullName: user.FullName,
            Email: user.Email!,
            Username: user.UserName!,
            AccessToken: tokens.AccessToken);
    }
}