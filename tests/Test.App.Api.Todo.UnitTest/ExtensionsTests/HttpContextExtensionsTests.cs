using System.Security.Claims;
using App.Api.Todo.Extensions;

namespace Test.App.Api.Todo.UnitTest.ExtensionsTests;

public class HttpContextExtensionsTests
{
    [Fact]
    public void GetEmail_ReturnsEmail_WhenEmailClaimExists()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Email, "user@example.com") };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var email = principal.GetEmail();

        // Assert
        Assert.Equal("user@example.com", email);
    }

    [Fact]
    public void GetEmail_ReturnsEmpty_WhenEmailClaimNotPresent()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        var email = principal.GetEmail();

        // Assert
        Assert.Equal(string.Empty, email);
    }

    [Fact]
    public void GetName_ReturnsName_WhenNameClaimExists()
    {
        // Arrange
        var claims = new[] { new Claim("name", "John Doe") };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var name = principal.GetName();

        // Assert
        Assert.Equal("John Doe", name);
    }

    [Fact]
    public void GetName_ReturnsEmpty_WhenNameClaimNotPresent()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        var name = principal.GetName();

        // Assert
        Assert.Equal(string.Empty, name);
    }
}
