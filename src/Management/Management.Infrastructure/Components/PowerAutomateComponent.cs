using ELTEKAps.Management.ApplicationServices.Components;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ELTEKAps.Management.Infrastructure.Components;
public class PowerAutomateComponent : IPowerAutomateComponent
{

    private readonly HttpClient _httpClient;
    private const string ContentTypeJson = "application/json";

    public PowerAutomateComponent(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient(Constants.HttpClientNames.PowerAutomate);
    }

    public async Task Notify(string recipientEmail, string username, string taskTitle)
    {
        var notifyRecipientUrl = $"?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=iQtV9-f07K8Pyy9UQGtZXyg-bwiEImGbw1g7vC2ZLNw";
        var powerAutomateRecipientRequest = new PowerAutomateRecipientRequest(recipientEmail, username, taskTitle);

        var powerAutomateRecipientRequestBody = JsonSerializer.Serialize(powerAutomateRecipientRequest);

        using var httpResponse = await _httpClient.PostAsync(notifyRecipientUrl, new StringContent(powerAutomateRecipientRequestBody, Encoding.UTF8, ContentTypeJson));
    }
}

public class PowerAutomateRecipientRequest
{
    [JsonPropertyName("email")] public string Email { get; set; }

    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("task")] public string Task { get; set; }

    public PowerAutomateRecipientRequest(string email, string username, string task)
    {
        Email = email;
        Username = username;
        Task = task;
    }
}
