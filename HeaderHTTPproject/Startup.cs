using System;
using System.IO;
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
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/index.html");
                    return Task.CompletedTask;
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

                    // Read the "results.html" file from the "wwwroot" folder
                    string resultsHtmlTemplatePath = Path.Combine(env.WebRootPath, "results.html");
                    string resultsHtmlTemplate = await File.ReadAllTextAsync(resultsHtmlTemplatePath);

                    // Replace the placeholder with the actual results
                    string finalResultsHtml = string.Format(resultsHtmlTemplate, resultsHtml);
                    await context.Response.WriteAsync(finalResultsHtml);
                });
            });
        }
    }
}
