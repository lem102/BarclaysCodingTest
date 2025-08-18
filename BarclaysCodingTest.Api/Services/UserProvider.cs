using System.Security.Claims;

namespace BarclaysCodingTest.Api.Services;

public class UserProvider(IHttpContextAccessor httpContextAccessor) : IUserProvider
{
    public Guid GetCurrentUserId()
    {
        var httpContext = httpContextAccessor.HttpContext;

        var userIdClaim = httpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier);

        var nullableUserId = userIdClaim?.Value;

        return nullableUserId switch {
            string userId => Guid.Parse(userId),
            _ => Guid.Empty,
        };
    }
}
