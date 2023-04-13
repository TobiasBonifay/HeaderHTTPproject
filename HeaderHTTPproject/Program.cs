using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HeaderHTTPproject
{
    /**
     * This is the entry point for the application.
     */
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseKestrel()
                        .UseStartup<Startup>()
                        .UseWebRoot("wwwroot");
                })
                .Build()
                .RunAsync();
        }
    }
}