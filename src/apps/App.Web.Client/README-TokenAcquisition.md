# Microsoft.Identity.Web Token Acquisition Implementation

This implementation provides a complete solution for acquiring and using access tokens with Microsoft.Identity.Web to call downstream APIs.

## Overview

The solution includes:

1. **CustomAuthentication.cs** - Enhanced authentication setup with proper token acquisition
2. **TokenService.cs** - Service for acquiring and caching access tokens
3. **ExampleTodoController.cs** - Example controller showing how to use tokens to call downstream APIs

## Key Features

✅ **Proper Token Acquisition**: Uses Microsoft.Identity.Web's `ITokenAcquisition` service
✅ **Token Caching**: Automatically caches tokens to improve performance
✅ **Error Handling**: Comprehensive error handling with logging
✅ **Security Best Practices**: Follows Azure security guidelines
✅ **Easy Integration**: Simple service injection pattern

## How It Works

### 1. Authentication Setup

The `AddCustomAuthentication` method configures:

- Microsoft.Identity.Web for Azure AD authentication
- Token acquisition for downstream API calls
- In-memory token caching
- Custom event handlers for token management

### 2. Token Acquisition Events

#### OnAuthorizationCodeReceived

```csharp
private static async Task OnAuthorizationCodeReceivedFunc(AuthorizationCodeReceivedContext context)
{
    // Acquires access token using Microsoft.Identity.Web
    var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(scopes, user: context.Principal);

    // Caches token for quick retrieval
    memoryCache.Set(cacheKey, accessToken, TimeSpan.FromMinutes(55));
}
```

#### OnTokenValidated

- Validates user principal and identity
- Creates user session management
- Adds custom claims (roles, session info)

### 3. Token Service Usage

```csharp
// Inject the token service
public TodoController(ITokenService tokenService, HttpClient httpClient)
{
    _tokenService = tokenService;
    _httpClient = httpClient;
}

// Acquire token and call API
var accessToken = await _tokenService.GetAccessTokenAsync(User, cancellationToken);
_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
var response = await _httpClient.GetAsync("https://your-api.com/endpoint");
```

## Configuration Required

### appsettings.json

```json
{
  "AzureEntra": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "CallbackPath": "/signin-oidc"
  },
  "TodoApi": {
    "Scopes": "api://your-api-client-id/access_as_user"
  }
}
```

## Benefits

1. **Simplified Token Management**: No need to manually handle token acquisition
2. **Performance Optimized**: Automatic token caching reduces API calls
3. **Error Resilient**: Graceful error handling prevents authentication failures
4. **Secure**: Follows Microsoft security best practices
5. **Maintainable**: Clean separation of concerns

## Important Notes

- Tokens are cached for 55 minutes (before expiration)
- Failed token acquisition doesn't break authentication flow
- All errors are logged for debugging
- Service supports both immediate and cached token retrieval

## Usage Example

```csharp
[HttpGet]
public async Task<IActionResult> GetData()
{
    var token = await _tokenService.GetAccessTokenAsync(User);
    if (token == null) return Unauthorized();

    _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

    var response = await _httpClient.GetAsync("https://api.example.com/data");
    return Ok(await response.Content.ReadAsStringAsync());
}
```

This implementation replaces manual token acquisition code and provides a robust, production-ready solution for calling downstream APIs with proper authentication.
