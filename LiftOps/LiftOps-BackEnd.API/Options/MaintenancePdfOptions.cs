namespace LiftOps_BackEnd.API.Options;

public class MaintenancePdfOptions
{
    public const string SectionName = "Maintenance";

    /// <summary>Validity window for shareable PDF download query tokens (HMAC-protected via Data Protection).</summary>
    public int PdfAccessTokenLifetimeMinutes { get; set; } = 15;
}
