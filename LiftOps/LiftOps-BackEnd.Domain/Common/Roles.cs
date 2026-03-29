namespace LiftOps_BackEnd.Domain.Common;

public static class Roles
{
    public const string Manager = "Manager";
    public const string InstallationAdmin = "InstallationAdmin";
    public const string MaintenanceAdmin = "MaintenanceAdmin";
    public const string InventoryAdmin = "InventoryAdmin";
    public const string FinanceAdmin = "FinanceAdmin";
    public const string FaultsAdmin = "FaultsAdmin";
    public const string Technician = "Technician";

    public static readonly IReadOnlyList<string> AllRoles = new[]
    {
        Manager,
        InstallationAdmin,
        MaintenanceAdmin,
        InventoryAdmin,
        FinanceAdmin,
        FaultsAdmin,
        Technician
    };
}
