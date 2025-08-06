using System.Net.Http.Headers;
using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Features.Todolist.Data;
using App.Api.Todo.Features.Todos.Data;
using App.Api.Todo.Features.Todotask.Data;
using App.Api.Todo.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Test.App.Api.Todo.IntegrationTest.Fixture;

namespace Test.App.Todo.Integration.Helper
{
    public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly IServiceScope _scope;
        protected readonly HttpClient Client;
        protected readonly TodoContext TodoContext;

        public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();
            TodoContext = _scope.ServiceProvider.GetRequiredService<TodoContext>();
            Client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost") // optional
            });
        }

        protected ITagRepository TagRepository
        {
            get
            {
                return _scope.ServiceProvider.GetRequiredService<ITagRepository>();
            }
        }

        protected ITodoTaskRepository TodoTaskRepository
        {
            get
            {
                return _scope.ServiceProvider.GetRequiredService<ITodoTaskRepository>();
            }
        }

        protected ITodolistRepository TodoListRepository
        {
            get
            {
                return _scope.ServiceProvider.GetRequiredService<ITodolistRepository>();
            }
        }

        protected ITodoRepository TodoRepository
        {
            get
            {
                return _scope.ServiceProvider.GetRequiredService<ITodoRepository>();
            }
        }

        protected void AuthenticateAsUser(string userId, params string[] additionalRoles)
        {
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test", userId);
        }

        /// <summary>
        /// Clears any authentication for testing unauthorized scenarios
        /// </summary>
        protected void ClearAuthentication()
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }

        /// <summary>
        /// Authenticates with a real JWT Bearer token (for more realistic testing)
        /// </summary>
        protected void AuthenticateWithBearerToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
