using MathSite.Common.ApiServiceRequester.Abstractions;

namespace MathSite.Api.Server.Infrastructure
{
    public class ExtendedAuthData : AuthConfig
    {
        public string Issuer { get; set; }
        public string Key { get; set; }
        public string Audience { get; set; }
        public int Lifetime { get; set; }
    }
}