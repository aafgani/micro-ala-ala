namespace App.Common.Domain.Auth;

public record TokenResult(string AccessToken, TimeSpan CacheDuration);
