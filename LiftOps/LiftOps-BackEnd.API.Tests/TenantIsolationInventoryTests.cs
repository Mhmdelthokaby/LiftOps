using System.Net;
using System.Net.Http.Json;
using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace LiftOps_BackEnd.API.Tests;

public sealed class TenantIsolationInventoryTests : IClassFixture<EmergencyWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly Guid _tenantA = Guid.NewGuid();
    private readonly Guid _tenantB = Guid.NewGuid();
    private readonly Guid _itemId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    public TenantIsolationInventoryTests(EmergencyWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        db.Categories.Add(new Category
        {
            Id = _categoryId,
            CompanyId = _tenantA,
            Name = "Electrical"
        });

        db.InventoryItems.Add(new InventoryItem
        {
            Id = _itemId,
            CompanyId = _tenantA,
            Name = "Cable",
            ItemNumber = "CAB-100",
            CategoryId = _categoryId,
            UnitPrice = 10m,
            StockQuantity = 20,
            SupplierName = "Supplier A",
            AddedByAdminName = "seed"
        });

        db.SaveChanges();
    }

    [Fact]
    public async Task Update_ByDifferentTenant_ReturnsBadRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/Inventory/update/{_itemId}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer",
            IntegrationTestJwt.CreateAccessToken(_tenantB, Roles.InventoryAdmin));
        request.Content = JsonContent.Create(new
        {
            name = "Cable Updated",
            itemNumber = "CAB-100",
            categoryId = _categoryId,
            unitPrice = 12m,
            stockQuantity = 22,
            supplierName = "Supplier A"
        });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ListAll_ByDifferentTenant_DoesNotContainOtherTenantItem()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Inventory/all");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer",
            IntegrationTestJwt.CreateAccessToken(_tenantB, Roles.InventoryAdmin));

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain(_itemId.ToString(), payload, StringComparison.OrdinalIgnoreCase);
    }
}
