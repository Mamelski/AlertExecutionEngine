using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AlertExecutionEngine.AlertsService.Http.Configuration;
using AlertExecutionEngine.AlertsService.Http.Models;
using AlertExecutionEngine.Domain.Contracts;
using DomainModels = AlertExecutionEngine.Domain.Contracts.Models;

namespace AlertExecutionEngine.AlertsService.Http;

// This class is a client for alerts http api.
// Naming maybe be not perfect since it serves as a client for alerts http api, but from domain point of view
// it is a external alerts service (hard to chose name for something that is both client and a service, and it is
// up to team/project naming convention and practices).
public class HttpAlertsService : IExternalAlertsService
{
    private readonly HttpAlertsServiceConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public HttpAlertsService(HttpClient httpClient, HttpAlertsServiceConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<DomainModels.Metric> GetMetric(string metricName)
    {
        var url = Path.Combine(_configuration.BaseUrl, _configuration.GetMetricEndpoint);
        var query = $"?target={metricName}";

        var response = await _httpClient.GetAsync($"{url}{query}");
        response.EnsureSuccessStatusCode();

        var metric = await response.Content.ReadFromJsonAsync<Metric>();
        return metric!.ToDomainModel();
    }

    public async Task<IList<DomainModels.Alert>> GetAllAlerts()
    {
        var url = Path.Combine(_configuration.BaseUrl, _configuration.GetAllAlertsEndpoint);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var alerts = await response.Content.ReadFromJsonAsync<IEnumerable<Alert>>();
        return alerts!.Select(alert => alert.ToDomainModel()).ToList();
    }

    public async Task SendNotification(DomainModels.Notification notification)
    {
        var url = Path.Combine(_configuration.BaseUrl, _configuration.NotifyEndpoint);

        var notificationJson = JsonSerializer.Serialize(notification.ToAlertServiceModel());
        var content = new StringContent(notificationJson, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
    }

    public async Task ResolveAlert(string alertName)
    {
        var url = Path.Combine(_configuration.BaseUrl, _configuration.ResolveEndpoint);

        // I use anonymous object but this can be record instead (ResolveName? Settlement? Resolve?)
        var resolveJson = JsonSerializer.Serialize(new {alertName});
        var content = new StringContent(resolveJson, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
    }
}