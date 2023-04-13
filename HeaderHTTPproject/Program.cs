using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HeaderHTTPproject
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseKestrel()
                        .UseStartup<Startup>()
                        .UseWebRoot("wwwroot");
                })
                .Build();

            await host.RunAsync();
        }
    }
}
