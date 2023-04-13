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

            // Run the host in a separate task
            var hostTask = host.RunAsync();

            // Get the URLs
            var serverUrls = Startup.MyNotSensitivePageUrls();

            // Run Question 1
            Console.WriteLine("Question 1");
            await Question1.Run(serverUrls);
            Console.WriteLine("--------------------");

            // Run Question 2
            Console.WriteLine("\nQuestion 2");
            await Question2.Run(serverUrls);
            Console.WriteLine("--------------------");

            // Run Question 3
            Console.WriteLine("\nQuestion 3");
            await TestScenarios.Scenario1();
            await TestScenarios.Scenario2();
            await TestScenarios.Scenario3();
            await TestScenarios.Scenario4();
            Console.WriteLine("--------------------");

            Console.WriteLine("End");
            
            // Wait for the host to finish running
            await hostTask;
        }
    }
}