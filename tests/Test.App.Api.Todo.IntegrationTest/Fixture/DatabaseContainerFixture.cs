using App.Api.Todo.Models;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using Test.App.Api.Todo.IntegrationTest.Helper;
using Testcontainers.PostgreSql;
using Task = System.Threading.Tasks.Task;

namespace Test.App.Api.Todo.IntegrationTest.Fixture
{
    public class DatabaseContainerFixture
    {
        private const string DbName = "testdb";
        private const string Username = "postgres";
        private const string Password = "postgres";
        private const ushort Port = 5432;
        public PostgreSqlContainer _dbContainer { get; private set; } = null!;
        public string ConnectionString => $"Host=127.0.0.1;Port={_dbContainer.GetMappedPublicPort(Port)};Database={DbName};Username={Username};Password={Password}";

        public async Task DisposeAsync() => await _dbContainer.DisposeAsync();

        public async Task InitializeAsync()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:16")
                .WithUsername(Username)
                .WithPassword(Password)
                .WithDatabase(DbName)
                .WithPortBinding(Port, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(Port))
                .Build();

            await _dbContainer.StartAsync();

            var hostPort = _dbContainer.GetMappedPublicPort(Port);

            // Wait until PostgreSQL is available
            await WaitUntilDatabaseAvailableAsync("127.0.0.1", hostPort);

            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            using (var context = new TodoContext(options))
            {
                await context.Database.EnsureCreatedAsync();
                await SeedDataAsync(context);
            }
        }

        private async Task WaitUntilDatabaseAvailableAsync(string host, int port)
        {
            var retries = 10;
            var delay = TimeSpan.FromSeconds(2);

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    using var tcpClient = new TcpClient();
                    await tcpClient.ConnectAsync(host, port);
                    if (tcpClient.Connected)
                    {
                        Console.WriteLine($"✅ Database server available at {host}:{port}");
                        return;
                    }
                }
                catch
                {
                    Console.WriteLine($"⏳ Waiting for database server at {host}:{port}...");
                }
                await Task.Delay(delay);
            }
            throw new Exception($"❌ Could not connect to the database server at {host}:{port} within the time limit.");
        }

        private Task SeedDataAsync(TodoContext context) => Task.CompletedTask;
    }
}
