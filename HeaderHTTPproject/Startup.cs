using System.Text;
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
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var indexPath = Path.Combine(env.WebRootPath, "index.html");
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync(indexPath);
                });

                endpoints.MapPost("/analyze", async context =>
                {
                    var urls = await GetUrlsFromFormAsync(context);
                    var (serverCounts, errors) = await GetServerCountsAsync(urls);
                    var resultsHtml = GenerateResultsHtml(serverCounts, urls.Count);
                    var errorsHtml = GenerateErrorsHtml(errors);

                    var resultsHtmlTemplatePath = Path.Combine(env.WebRootPath, "results.html");
                    var resultsHtmlTemplate = await File.ReadAllTextAsync(resultsHtmlTemplatePath);

                    var finalResultsHtml = resultsHtmlTemplate.Replace("{0}", resultsHtml);
                    finalResultsHtml = finalResultsHtml.Replace("{1}", errorsHtml);
                    
                    Console.WriteLine("finalResultsHtml: " + finalResultsHtml);
                    await context.Response.WriteAsync(finalResultsHtml);
                });
            });
        }

        private async Task<List<string>> GetUrlsFromFormAsync(HttpContext context)
        {
            var form = await context.Request.ReadFormAsync();
            return form.Keys.SelectMany(key => form[key].ToString().Split(',')).ToList();
        }

        private async Task<(Dictionary<string, int>, List<string>)> GetServerCountsAsync(List<string> urls)
        {
            var httpClient = new HttpClient();
            var serverCounts = new Dictionary<string, int>();
            var errors = new List<string>();

            foreach (var url in urls)
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    var server = response.Headers.Server.ToString();
                    if (!serverCounts.ContainsKey(server)) serverCounts[server] = 0;
                    serverCounts[server]++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
                    errors.Add($"Error fetching headers for {url}: {ex.Message}");
                }
            }
            return (serverCounts, errors);
        }
        
        private string GenerateResultsHtml(Dictionary<string, int> serverCounts, int totalCount)
        {
            StringBuilder serverCountsHtml = new StringBuilder();
            foreach (var serverCount in serverCounts)
            {
                string server = serverCount.Key;
                int count = serverCount.Value;
                double percentage = (count / (double)totalCount) * 100;
                serverCountsHtml.Append($"<tr><td>{server}</td><td>{count}</td><td>{percentage:N2}%</td></tr>");
            }
            return serverCountsHtml.ToString();
        }
        
        private string GenerateErrorsHtml(List<string> errors)
        {
            StringBuilder errorsHtml = new StringBuilder();
            foreach (var error in errors) errorsHtml.Append($"<li>{error}</li>");
            return errorsHtml.ToString();
        }
    }
}
