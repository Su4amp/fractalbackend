using System.Security.Claims;

namespace Application.Common.Interfaces.Authorization;

public interface IUserAuthorizationService
{
    Guid GetUserIdFromJwtToken(ClaimsPrincipal user);
}