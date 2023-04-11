using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
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
                string css = @"
                    body {
                        font-family: Arial, sans-serif;
                        max-width: 800px;
                        margin: 0 auto;
                    }
                    table {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    th, td {
                        border: 1px solid #ccc;
                        padding: 8px;
                        text-align: left;
                    }
                    th {
                        background-color: #f2f2f2;
                        font-weight: bold;
                    }
                    a {
                        display: inline-block;
                        margin-top: 20px;
                        text-decoration: none;
                        color: #0099ff;
                        font-weight: bold;
                    }";

                string formHtml = @"
                    <html>
                        <head>
                            <style>
                                {0}
                            </style>
                        </head>
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
                    </html>";

                string resultsHtmlTemplate = @"
                    <html>
                        <head>
                            <style>
                                {0}
                            </style>
                        </head>
                        <body>
                            <h1>Web Server Analysis</h1>
                            <table>
                                <tr>
                                    <th>Web Server</th>
                                    <th>Count</th>
                                    <th>Percentage</th>
                                </tr>
                                {1}
                            </table>
                            <a href=""/"">Analyze more URLs</a>
                        </body>
                    </html>";

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(string.Format(formHtml, css));
                });

                endpoints.MapPost("/analyze", async context =>
                {
                    var form = await context.Request.ReadFormAsync();
                    var urls = form.Keys.Select(key => form[key]).ToList();

                    var httpClient = new HttpClient();
                    Dictionary<string, int> serverCounts = new Dictionary<string, int>();

                    foreach (var url in urls)
                    {
                        try
                        {
                            var response = await httpClient.GetAsync(url);
                            string server = response.Headers.Server.ToString();
                            if (!serverCounts.ContainsKey(server))
                            {
                                serverCounts[server] = 0;
                            }
                            serverCounts[server]++;
                        }
                        catch (Exception ex)
                        {
                            await context.Response.WriteAsync($"Error fetching headers for {url}: {ex.Message}\n\n");
                        }
                    }

                    double total = urls.Count;
                    string resultsHtml = "";
                    foreach (var serverCount in serverCounts)
                    {
                        string server = serverCount.Key;
                        int count = serverCount.Value;
                        double percentage = (count / total) * 100;
                        resultsHtml += $"<tr><td>{server}</td><td>{count}</td><td>{percentage:N2}%</td></tr>";
                    }

                    await context.Response.WriteAsync(string.Format(resultsHtmlTemplate, css, resultsHtml));
                });
            });
        }
    }
}

