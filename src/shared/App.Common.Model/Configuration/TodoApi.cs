namespace App.Common.Domain.Configuration;

public class ExternalApi
{
    public string BaseUrl { get; set; }
    public string Scopes { get; set; }
}

public class TodoApi : ExternalApi
{
}
