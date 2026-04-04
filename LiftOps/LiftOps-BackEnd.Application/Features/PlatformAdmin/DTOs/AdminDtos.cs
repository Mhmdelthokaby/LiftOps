namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;

// Dashboard
public record PlatformDashboardDto(
    int TotalCompanies,
    int ActiveSubscriptions,
    decimal MonthlyRevenue,
    int TrialCompanies,
    List<RevenueTrendDto> RevenueTrend,
    List<PlanDistributionDto> CompaniesByPlan);

public record RevenueTrendDto(string Month, decimal Revenue);
public record PlanDistributionDto(string PlanName, int Count);

// Companies
public record CompanyListDto(
    Guid Id,
    string Name,
    string? ContactEmail,
    string Status,
    string? PlanName,
    int CurrentUserCount,
    int MaxUsers,
    int CurrentElevatorCount,
    int MaxElevators,
    DateTime CreatedAt);

public record SubscriptionDto(
    Guid Id,
    Guid CompanyId,
    Guid PlanId,
    string PlanName,
    string Status,
    DateTime CurrentPeriodStart,
    DateTime CurrentPeriodEnd,
    string BillingCycle);

public record CompanyDetailDto(
    Guid Id,
    string Name,
    string? ContactEmail,
    string? ContactPhone,
    string Status,
    DateTime CreatedAt,
    SubscriptionDto? Subscription,
    PlanLimitsDto? PlanLimits);

public record PlanLimitsDto(
    int MaxUsers,
    int MaxElevators,
    int MaxMaintenanceContracts,
    int MaxInstallationProjects);

public record CreateCompanyRequest(string Name, string ContactEmail, Guid PlanId);
public record SuspendCompanyRequest(string Reason);
public record ExtendTrialRequest(int Days);

public record CreateCompanyResponseDto(CompanyDetailDto Company, string InitialPassword);

// Plans
public record PlanDto(
    Guid Id,
    string Name,
    string Code,
    decimal MonthlyPrice,
    decimal YearlyPrice,
    int TrialDays,
    bool IsActive,
    int MaxUsers,
    int MaxElevators,
    int MaxMaintenanceContracts,
    int MaxInstallationProjects,
    bool AllowEmergencyModule,
    bool AllowFaultsModule,
    bool AllowFinanceModule,
    bool AllowInventoryModule,
    bool AllowApiAccess);

public record CreatePlanRequest(
    string Name,
    string Code,
    decimal MonthlyPrice,
    decimal YearlyPrice,
    int TrialDays,
    int MaxUsers,
    int MaxElevators,
    int MaxMaintenanceContracts,
    int MaxInstallationProjects,
    bool AllowEmergencyModule,
    bool AllowFaultsModule,
    bool AllowFinanceModule,
    bool AllowInventoryModule,
    bool AllowApiAccess);

public record UpdatePlanRequest(
    string Name,
    string Code,
    decimal MonthlyPrice,
    decimal YearlyPrice,
    int TrialDays,
    bool IsActive,
    int MaxUsers,
    int MaxElevators,
    int MaxMaintenanceContracts,
    int MaxInstallationProjects,
    bool AllowEmergencyModule,
    bool AllowFaultsModule,
    bool AllowFinanceModule,
    bool AllowInventoryModule,
    bool AllowApiAccess);

// Subscriptions & Users
public record SubscriptionListDto(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    string PlanName,
    string Status,
    string BillingCycle,
    DateTime CurrentPeriodStart,
    DateTime CurrentPeriodEnd,
    int DaysRemaining,
    decimal Amount);

public record GlobalUserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    Guid? CompanyId,
    string CompanyName,
    string Role,
    DateTime? LastLogin,
    bool IsActive);

public record PaginatedResultDto<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record ChangePlanRequest(Guid NewPlanId);
public record ImpersonateUserRequest(Guid UserId);

public record ImpersonateUserResponseDto(
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpiry,
    string Name,
    string Email,
    IReadOnlyList<string> Roles);
