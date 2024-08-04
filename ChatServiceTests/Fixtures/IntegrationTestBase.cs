using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ChatServiceTests.Fixtures;

public class IntegrationTestBase : IClassFixture<ChatServiceFactory>
{
    protected readonly ChatServiceFactory Factory;
    protected readonly HttpClient Client;
    

    protected IntegrationTestBase(ChatServiceFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }
    
    protected T GetService<T>() where T : class
    {
        return Factory.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
    }
    
    
    
}