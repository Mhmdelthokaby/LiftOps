using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace LiftOps_BackEnd.API.Tests;

/// <summary>
/// Injects integration-test configuration (InMemory DB + JWT signing key) so the same host as production
/// validates Bearer tokens built by <see cref="IntegrationTestJwt"/>.
/// </summary>
public sealed class EmergencyWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseInMemoryDatabase"] = "true",
                ["Jwt:Key"] = IntegrationTestJwt.TestKey,
                ["Jwt:Issuer"] = IntegrationTestJwt.TestIssuer,
                ["Jwt:Audience"] = IntegrationTestJwt.TestAudience
            });
        });
    }
}
