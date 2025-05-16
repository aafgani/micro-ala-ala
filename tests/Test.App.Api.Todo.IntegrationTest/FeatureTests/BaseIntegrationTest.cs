using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Test.App.Api.Todo.IntegrationTest.Fixture;

namespace Test.App.Api.Todo.IntegrationTest.FeatureTests
{
    public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly IServiceScope _scope;
        protected readonly HttpClient Client;
        protected readonly TodoContext TodoContext;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();
            TodoContext = _scope.ServiceProvider.GetRequiredService<TodoContext>();
            Client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost") // optional
            });
        }
        
        protected ITagRepository TagRepository { 
            get 
            {
                return _scope.ServiceProvider.GetRequiredService<ITagRepository>();
            } 
        }
    }
}
