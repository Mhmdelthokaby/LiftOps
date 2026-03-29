using System.Text;
using LiftOps_BackEnd.API.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace LiftOps_BackEnd.API.Security;

/// <summary>
/// Creates and validates time-limited tokens for GET /api/maintenance/visit/{visitId}/pdf?pdfToken=...
/// Payload is protected with ASP.NET Core Data Protection (not plain HMAC; keys should be persisted in production).
/// </summary>
public class MaintenanceVisitPdfAccessService
{
    private readonly IDataProtector _protector;
    private readonly MaintenancePdfOptions _options;

    public MaintenanceVisitPdfAccessService(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<MaintenancePdfOptions> options)
    {
        _protector = dataProtectionProvider.CreateProtector("LiftOps.MaintenanceVisitPdf.v1");
        _options = options.Value;
    }

    public string CreateToken(Guid visitId)
    {
        var exp = DateTimeOffset.UtcNow.AddMinutes(_options.PdfAccessTokenLifetimeMinutes);
        var payload = $"{visitId:N}|{exp.ToUnixTimeSeconds()}";
        var protectedBytes = _protector.Protect(Encoding.UTF8.GetBytes(payload));
        return WebEncoders.Base64UrlEncode(protectedBytes);
    }

    public bool TryValidateToken(string token, Guid visitId, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(token))
        {
            error = "missing_token";
            return false;
        }

        try
        {
            var bytes = WebEncoders.Base64UrlDecode(token);
            var payload = Encoding.UTF8.GetString(_protector.Unprotect(bytes));
            var parts = payload.Split('|', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2 || !Guid.TryParse(parts[0], out var id) || id != visitId)
            {
                error = "invalid_token";
                return false;
            }

            if (!long.TryParse(parts[1], out var expUnix))
            {
                error = "invalid_token";
                return false;
            }

            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expUnix)
            {
                error = "expired_token";
                return false;
            }

            return true;
        }
        catch
        {
            error = "invalid_token";
            return false;
        }
    }

    public DateTimeOffset GetTokenExpiryUtc()
        => DateTimeOffset.UtcNow.AddMinutes(_options.PdfAccessTokenLifetimeMinutes);
}
