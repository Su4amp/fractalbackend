using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Users;
using Application.Services.Authentication;
using Application.Services.Authorization;
using Application.Services.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();

        return services;
    }
}