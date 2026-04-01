using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LiftOps_BackEnd.API.Tests;

internal static class IntegrationTestJwt
{
    internal const string TestIssuer = "LiftOpsBackEnd";
    internal const string TestAudience = "LiftOpsBackEnd";
    internal const string TestKey = "#fevbgflvmfLift#223ffOps#omwsdaSecretKetgdhvkeovnw#sfavdtctwqadvoplmnyv";

    public static string CreateAccessToken(params string[] roles)
        => CreateAccessToken(Guid.NewGuid(), roles);

    public static string CreateAccessToken(Guid companyId, params string[] roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Email, "integration-test@example.com"),
            new(ClaimTypes.Name, "Integration Test"),
            new("company_id", companyId.ToString())
        };
        foreach (var r in roles)
            claims.Add(new Claim(ClaimTypes.Role, r));

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
