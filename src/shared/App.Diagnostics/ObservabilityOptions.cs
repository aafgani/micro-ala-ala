using System;

namespace App.Diagnostics;

public class ObservabilityOptions
{
    public const string SectionKey = nameof(Diagnostics);
    public string Provider { get; set; } = ObservabilityProvider.None;
    public string? Endpoint { get; set; }

}

public class ObservabilityProvider
{
    public const string None = nameof(None);
    public const string OpenTelemetry = nameof(OpenTelemetry);
    public const string Nlog = nameof(Nlog);
}
