using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

internal class KernelSettings
{
    public const string SectionName = "KernelSettings";

    public string ServiceType { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;

    public string DeploymentId { get; set; } = string.Empty;

    public string ModelId { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string OrgId { get; set; } = string.Empty;

    public LogLevel? LogLevel { get; set; } = Microsoft.Extensions.Logging.LogLevel.Warning;

    public string SystemPrompt { get; set; } = "You are a friendly, intelligent, and curious assistant who is good at conversation.";

}
