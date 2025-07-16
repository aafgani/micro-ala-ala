using App.Common.Domain;
using App.Common.Infrastructure.KeyVault;
using Azure.Security.KeyVault.Secrets;
using Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using App.Common.Domain.Configuration;

namespace Test.App.Common.Infrastructure.UnitTest.KeyVaultTests
{
    public class KeyVaultSecretServiceTests
    {
        private readonly string _keyVaultUrl = "https://test.vault.azure.net/";
        private readonly Mock<ILogger<KeyVaultSecretService>> _mockLogger = new();
        private readonly Mock<IKeyVaultClient> _mockClient = new();
        private readonly IOptions<EntraConfiguration> _entraOptions;
        private readonly IOptions<CustomRetryPolicy> _retryOptions;

        public KeyVaultSecretServiceTests()
        {
            _entraOptions = Options.Create(new EntraConfiguration
            {
                TenantId = "tenant",
                ClientId = "client",
                ClientSecret = "secret"
            });
            _retryOptions = Options.Create(new CustomRetryPolicy
            {
                MaxRetryCount = 2,
                DelaySeconds = 1
            });
        }

        [Fact]
        public void Constructor_InitializesDependencies()
        {
            // Arrange & Act
            var service = new KeyVaultSecretService(_mockClient.Object, _retryOptions, _mockLogger.Object);

            // Assert
            service.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetSecretAsync_ReturnsSecretValue()
        {
            // Arrange
            var secretName = "mySecret";
            var secretValue = "secretValue";
            var mockResponse = Mock.Of<Response<KeyVaultSecret>>(r =>
                r.Value == new KeyVaultSecret(secretName, secretValue)
            );

            _mockClient
                .Setup(c => c.GetSecretAsync(secretName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);

            var service = new KeyVaultSecretService(_mockClient.Object, _retryOptions, _mockLogger.Object);

            // Act
            var result = await service.GetSecretAsync(secretName);

            // Assert
            result.ShouldBe(secretValue);
        }

        [Fact]
        public async Task GetSecretAsync_ThrowsAndLogsOnRequestFailedException()
        {
            // Arrange
            var secretName = "failSecret";
            var exception = new Azure.RequestFailedException(500, "Internal error");

            _mockClient
                .Setup(c => c.GetSecretAsync(secretName, It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var service = new KeyVaultSecretService(_mockClient.Object, _retryOptions, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<RequestFailedException>(() => service.GetSecretAsync(secretName));
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to retrieve secret")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(429, true)]
        [InlineData(500, true)]
        [InlineData(503, true)]
        [InlineData(404, false)]
        [InlineData(400, false)]
        public void IsTransientError_ReturnsExpected(int status, bool expected)
        {
            // Arrange
            var ex = new RequestFailedException(status, "error");
            var service = new KeyVaultSecretService(_mockClient.Object, _retryOptions, _mockLogger.Object);

            // Use reflection to invoke private method
            var method = typeof(KeyVaultSecretService)
                .GetMethod("IsTransientError", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var result = (bool)method.Invoke(service, new object[] { ex });

            // Assert
            result.ShouldBe(expected);
        }
    }
}