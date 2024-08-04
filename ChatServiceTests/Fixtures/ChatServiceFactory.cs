using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace ChatServiceTests.Fixtures;

public class ChatServiceFactory : WebApplicationFactory<Program>
{
   protected override IHostBuilder CreateHostBuilder()
   {
      return Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
      {
         webBuilder.UseStartup<TestStartup>();
      });
   }
}