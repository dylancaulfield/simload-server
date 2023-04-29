using Microsoft.AspNetCore.Http;

namespace SimLoad.Server.Common;

public interface IRequestContext
{
    Guid? UserId { get; }
    Guid? LoadGeneratorCredentialId { get; }
}

public class RequestContext : IRequestContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => GetUserId();
    public Guid? LoadGeneratorCredentialId => GetLoadGeneratorCredentialId();

    private Guid? GetUserId()
    {
        var userIdString =
            _httpContextAccessor.HttpContext?.User.Claims.First(c => c.Type == "userId").Value;
        if (Guid.TryParse(userIdString, out var userId)) return userId;

        return null;
    }

    private Guid? GetLoadGeneratorCredentialId()
    {
        var loadGeneratorIdString =
            _httpContextAccessor.HttpContext?.User.Claims.First(c => c.Type == "loadGeneratorCredentialId").Value;
        if (Guid.TryParse(loadGeneratorIdString, out var loadGeneratorId)) return loadGeneratorId;

        return null;
    }
}