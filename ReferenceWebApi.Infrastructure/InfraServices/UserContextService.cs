using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ReferenceWebApi.Application.Interfaces;

namespace ReferenceWebApi.Infrastructure.InfraServices
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => GetClaimValue(ClaimTypes.NameIdentifier);

        public string? Username => GetClaimValue(ClaimTypes.Name)
            ?? GetClaimValue("preferred_username") // For Azure AD/OAuth
            ?? GetHeaderValue("X-Username"); // Fallback to header

        public string? Email => GetClaimValue(ClaimTypes.Email);

        public string? IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        public IEnumerable Roles => _httpContextAccessor.HttpContext?.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value) ?? [];

        private string? GetClaimValue(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirst(claimType)?.Value;
        }

        private string? GetHeaderValue(string headerName)
        {
            return _httpContextAccessor.HttpContext?.Request
                .Headers[headerName].FirstOrDefault();
        }
    }
}
