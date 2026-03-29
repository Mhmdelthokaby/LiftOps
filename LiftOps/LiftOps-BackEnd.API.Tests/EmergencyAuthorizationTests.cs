using System.Net;
using System.Net.Http.Json;
using System.Text;
using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace LiftOps_BackEnd.API.Tests;

public sealed class EmergencyAuthorizationTests : IClassFixture<EmergencyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EmergencyAuthorizationTests(EmergencyWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        }
    }

    private static HttpRequestMessage GetWithRole(string path, string role)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer",
            IntegrationTestJwt.CreateAccessToken(role));
        return request;
    }

    private static HttpRequestMessage SendWithRole(HttpMethod method, string path, string role)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer",
            IntegrationTestJwt.CreateAccessToken(role));
        return request;
    }

    [Fact]
    public async Task GetAll_AsInventoryAdmin_ReturnsForbidden()
    {
        var response = await _client.SendAsync(GetWithRole("/api/Emergency", Roles.InventoryAdmin));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_AsFinanceAdmin_ReturnsForbidden()
    {
        var response = await _client.SendAsync(GetWithRole("/api/Emergency", Roles.FinanceAdmin));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetOpen_AsTechnician_ReturnsOk()
    {
        var response = await _client.SendAsync(GetWithRole("/api/Emergency/open", Roles.Technician));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_AsMaintenanceAdmin_ReturnsOk()
    {
        var response = await _client.SendAsync(GetWithRole("/api/Emergency", Roles.MaintenanceAdmin));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AsTechnician_ReturnsForbidden()
    {
        var id = Guid.NewGuid();
        var response = await _client.SendAsync(SendWithRole(HttpMethod.Delete, $"/api/Emergency/{id}", Roles.Technician));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_AsTechnician_ReturnsForbidden()
    {
        var id = Guid.NewGuid();
        var request = SendWithRole(HttpMethod.Put, $"/api/Emergency/{id}", Roles.Technician);
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AssignTechnician_AsTechnician_ReturnsForbidden()
    {
        var id = Guid.NewGuid();
        var request = SendWithRole(HttpMethod.Put, $"/api/Emergency/{id}/assign-technician", Roles.Technician);
        request.Content = JsonContent.Create(new { technicianId = Guid.NewGuid() });
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AssignTechnician_AsInstallationAdmin_ReturnsForbidden()
    {
        var id = Guid.NewGuid();
        var request = SendWithRole(HttpMethod.Put, $"/api/Emergency/{id}/assign-technician", Roles.InstallationAdmin);
        request.Content = JsonContent.Create(new { technicianId = Guid.NewGuid() });
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Resolve_AsFinanceAdmin_ReturnsForbidden()
    {
        var id = Guid.NewGuid();
        var request = SendWithRole(HttpMethod.Post, $"/api/Emergency/{id}/resolve", Roles.FinanceAdmin);
        request.Content = JsonContent.Create(new { notes = "done" });
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
