using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HeaderHTTPproject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseKestrel()
                        .UseStartup<Startup>();
                })
                .Build();

            await host.RunAsync();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(@"
                        <html>
                            <body>
                                <form action=""/analyze"" method=""POST"">
                                    <label for=""url1"">URL 1:</label>
                                    <input type=""text"" id=""url1"" name=""url1"" required><br>
                                    <label for=""url2"">URL 2:</label>
                                    <input type=""text"" id=""url2"" name=""url2"" required><br>
                                    <label for=""url3"">URL 3:</label>
                                    <input type=""text"" id=""url3"" name=""url3"" required><br>
                                    <label for=""url4"">URL 4:</label>
                                    <input type=""text"" id=""url4"" name=""url4"" required><br>
                                    <label for=""url5"">URL 5:</label>
                                    <input type=""text"" id=""url5"" name=""url5"" required><br>
                                    <input type=""submit"" value=""Analyze"">
                                </form>
                            </body>
                        </html>");
                });

                endpoints.MapPost("/analyze", async context =>
                {
                    var form = await context.Request.ReadFormAsync();
                    var urls = form.Keys.Select(key => form[key]).ToList();

                    var httpClient = new HttpClient();

                    foreach (var url in urls)
                    {
                        try
                        {
                            var response = await httpClient.GetAsync(url);
                            await context.Response.WriteAsync($"Headers for {url}:\n");
                            foreach (var header in response.Headers)
                            {
                                await context.Response.WriteAsync($"{header.Key}: {string.Join(", ", header.Value)}\n");
                            }
                            await context.Response.WriteAsync("\n");
                        }
                        catch (Exception ex)
                        {
                            await context.Response.WriteAsync($"Error fetching headers for {url}: {ex.Message}\n\n");
                        }
                    }
                });
            });
        }
    }
}
