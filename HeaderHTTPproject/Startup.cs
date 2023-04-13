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
        private static HttpClient HttpClient { get; } = new() { Timeout = TimeSpan.FromSeconds(5) };

        public static List<string> MyNotSensitivePageUrls()
        {
            return new List<string>
            {
                "http://google.fr",
                "http://www.reddit.com/",
                "http://twitch.fr/",
                "http://www.amazon.fr/",
                "http://docs.oracle.com/en/java/javase/19/index.html",
                "http://www.microsoft.com/",
                "http://discord.com/",
                "http://wikipedia.org",
            };
        }
        public static void ConfigureServices(IServiceCollection services)
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

                    // Question 1
                    var (serverCounts, errors) = await GetServerCountsAsync(urls);
                    var resultsHtml = GenerateResultsHtml(serverCounts, urls.Count);
                    var errorsHtml = GenerateErrorsHtml(errors);

                    // Question 2
                    var ages = new List<double>();
                    foreach (var url in urls)
                    {
                        var age = await GetPageAgeAsync2(url);
                        if (age.HasValue) ages.Add(age.Value);
                    }
                    var averageAge = CalculateAverageAge(ages);
                    var standardDeviation = CalculateStandardDeviation(ages, averageAge);

                    // Results for Question 2
                    var q2ResultsHtml = $"<p>Average Age: {averageAge:N2} seconds</p><p>Standard Deviation: {standardDeviation:N2} seconds</p>";

                    var resultsHtmlTemplatePath = Path.Combine(env.WebRootPath, "results.html");
                    var resultsHtmlTemplate = await File.ReadAllTextAsync(resultsHtmlTemplatePath);

                    var finalResultsHtml = resultsHtmlTemplate.Replace("{0}", resultsHtml);
                    finalResultsHtml = finalResultsHtml.Replace("{1}", errorsHtml);
                    finalResultsHtml = finalResultsHtml.Replace("{2}", q2ResultsHtml);

                    Console.WriteLine("finalResultsHtml: " + finalResultsHtml);
                    await context.Response.WriteAsync(finalResultsHtml);
                });
                
                endpoints.MapGet("/question3", async context =>
                {
                    await TestScenarios.Scenario1();
                    await TestScenarios.Scenario2();
                    await TestScenarios.Scenario3();
                    await TestScenarios.Scenario4();

                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Test scenarios executed.");
                });
            });
        }

        public static async Task<List<string>> GetUrlsFromFormAsync(HttpContext context)
        {
            var form = await context.Request.ReadFormAsync();
            return form.Keys.SelectMany(key => form[key].ToString().Split(',')).ToList();
        }

        public static async Task<(Dictionary<string, int>, List<string>)> GetServerCountsAsync(IEnumerable<string> urls)
        {
            var serverCounts = new Dictionary<string, int>();
            var errors = new List<string>();

            foreach (var url in urls)
            {
                try
                {
                    var response = await HttpClient.GetAsync(url);
                    var server = response.Headers.Server.ToString();
                    if (string.IsNullOrWhiteSpace(server)) server = "Unknown";
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

        public static string GenerateResultsHtml(Dictionary<string, int> serverCounts, int totalCount)
        {
            var serverCountsHtml = new StringBuilder();
            foreach (var (serverName, count) in serverCounts)
            {
                var percentage = count / (double)totalCount * 100;
                serverCountsHtml.Append($"<tr><td>{serverName}</td><td>{count}</td><td>{percentage:N2}%</td></tr>");
            }

            return serverCountsHtml.ToString();
        }

        public static string GenerateErrorsHtml(List<string> errors)
        {
            var errorsHtml = new StringBuilder();
            if (errors.Count == 0) return "";
            errorsHtml.Append("<h2>Errors</h2><ul>");
            foreach (var error in errors) errorsHtml.Append($"<li>{error}</li>");
            errorsHtml.Append("</ul>");
            return errorsHtml.ToString();
        }

        public static async Task<List<(double age, long contentLength, string contentType)>> GetPageAgesAsync2(IEnumerable<string> urls)
        {
            var results = new List<(double age, long contentLength, string contentType)>();

            foreach (var url in urls)
            {
                try
                {
                    var response = await HttpClient.GetAsync(url);
                    if (!response.Headers.Date.HasValue) continue;

                    var age = (DateTime.UtcNow - response.Headers.Date.Value).TotalSeconds;
                    var contentLength = response.Content.Headers.ContentLength ?? 0;
                    var contentType = response.Content.Headers.ContentType?.MediaType ?? "Unknown";

                    results.Add((age, contentLength, contentType));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
                }
            }

            return results;
        }

        public static async Task<double?> GetPageAgeAsync2(string url)
        {
            try
            {
                var response = await HttpClient.GetAsync(url);
                if (!response.Headers.Date.HasValue) return null;
                var age = (DateTime.UtcNow - response.Headers.Date.Value).TotalSeconds;
                return age;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
                return null;
            }
        }


        public static double CalculateAverageAge(List<double> ages)
        {
            return ages.Sum() / ages.Count;
        }

        public static double CalculateStandardDeviation(IReadOnlyCollection<double> ages, double averageAge)
        {
            var sum = ages.Sum(age => Math.Pow(age - averageAge, 2));
            return Math.Sqrt(sum / ages.Count);
        }
        
        public static string FormatBytes(long bytes)
        {
            string[] units = { "bytes", "KB", "MB", "GB" };
            double size = bytes;
            var unitIndex = 0;

            while (size > 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return $"{size:0.##} {units[unitIndex]}";
        }

    }
}
