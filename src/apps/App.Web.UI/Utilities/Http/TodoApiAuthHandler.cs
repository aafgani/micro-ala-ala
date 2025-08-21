using System.Net.Http.Headers;
using App.Web.UI.Utilities.Http.Token;
using static App.Web.UI.Utilities.Enums.Constant;

namespace App.Web.UI.Utilities.Http;

public class TodoApiAuthHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string[] _scopes;

    public TodoApiAuthHandler(ITokenService tokenService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
        _scopes = configuration["TodoApi:Scopes"]?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accountId = _httpContextAccessor.HttpContext?.User.FindFirst(CustomClaimTypes.MsalAccountId)?.Value;
        if (string.IsNullOrEmpty(accountId))
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
            {
                RequestMessage = request,
                ReasonPhrase = "Missing or invalid account ID for authentication."
            };
        }
        var token = await _tokenService.GetAccessTokenAsync(accountId, _scopes);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
