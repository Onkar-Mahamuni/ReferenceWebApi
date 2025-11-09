using System.Collections;

namespace ReferenceWebApi.Application.Interfaces
{
    public class IUserContextService
    {
        string? UserId { get; }
        string? Username { get; }
        string? Email { get; }
        string? IpAddress { get; }
        IEnumerable Roles { get; }
    }
}
