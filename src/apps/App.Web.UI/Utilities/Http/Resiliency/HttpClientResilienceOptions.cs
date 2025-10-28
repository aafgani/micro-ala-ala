using System;

namespace App.Web.UI.Utilities.Http.Resiliency;

public class HttpClientResilienceOptions
{
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);
    public int CircuitBreakerFailures { get; set; } = 5;
    public TimeSpan CircuitBreakerDelay { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
