using System;
using System.Collections.Generic;
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
                    var serverStats = new Dictionary<string, int>();
                    int totalCount = 0;

                    foreach (var url in urls)
                    {
                        try
                        {
                            var response = await httpClient.GetAsync(url);
                            if (response.Headers.Contains("Server"))
                            {
                                var serverName = response.Headers.GetValues("Server").FirstOrDefault();
                                if (!string.IsNullOrEmpty(serverName))
                                {
                                    totalCount++;
                                    if (serverStats.ContainsKey(serverName))
                                    {
                                        serverStats[serverName]++;
                                    }
                                    else
                                    {
                                        serverStats[serverName] = 1;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // Ignore any exceptions for this example.
                        }
                    }

                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync("<h3>Server Summary</h3>");
                    await context.Response.WriteAsync("<table><thead><tr><th>Server</th><th>Count</th><th>Percentage</th></tr></thead><tbody>");

                    foreach (var serverStat in serverStats)
                    {
                        double percentage = (double)serverStat.Value / totalCount * 100;
                        await context.Response.WriteAsync($"<tr><td>{serverStat.Key}</td><td>{serverStat.Value}</td><td>{percentage:0.00}%</td></tr>");
                    }

                    await context.Response.WriteAsync("</tbody></table>");
                    await context.Response.WriteAsync("<h3>Headers</h3>");

                    foreach (var url in urls)
                    {
                        try
                        {
                            var response = await httpClient.GetAsync(url);
                            await context.Response.WriteAsync($"<h4>Headers for {url}:</h4>");
                            await context.Response.WriteAsync("<pre>");
                            foreach (var header in response.Headers)
                            {
                                await context.Response.WriteAsync($"{header.Key}: {string.Join(", ", header.Value)}\n");
                            }
                            await context.Response.WriteAsync("</pre>");
                        }
                        catch (Exception ex)
                        {
                            await context.Response.WriteAsync($"<p>Error fetching headers for {url}: {ex.Message}</p>");
                        }
                    }

                    await context.Response.WriteAsync("</body></html>");
                });
            });
        }
    }
}
