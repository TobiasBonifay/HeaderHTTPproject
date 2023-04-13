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
                    var urls = await HtmlGenerator.GetUrlsFromForm(context);

                    // Question 1
                    var (serverCounts, errors) = await Calculation.GetServerCounts(urls);
                    var resultsHtml = HtmlGenerator.GenerateResultsHtml(serverCounts, urls.Count);
                    var errorsHtml = HtmlGenerator.GenerateErrorsHtml(errors);

                    // Question 2
                    var ages = new List<double>();
                    foreach (var url in urls)
                    {
                        var age = await Calculation.GetPageAge(url);
                        if (age.HasValue) ages.Add(age.Value);
                    }
                    var averageAge = Calculation.CalculateAverageAge(ages);
                    var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);

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
                
                endpoints.MapGet("/question1", async context =>
                {
                    var urls = BestWebsites.MyNotSensitivePageUrls();
                    var (serverCounts, errors) = await Calculation.GetServerCounts(urls);
                    var resultsHtml = HtmlGenerator.GenerateResultsHtml(serverCounts, urls.Count);
                    var errorsHtml = HtmlGenerator.GenerateErrorsHtml(errors);

                    var resultPageTemplatePath = Path.Combine(env.WebRootPath, "result-page-template.html");
                    var resultPageTemplate = await File.ReadAllTextAsync(resultPageTemplatePath);

                    var finalResultPage = resultPageTemplate.Replace("{pageTitle}", "Question 1")
                        .Replace("{resultSummary}", resultsHtml)
                        .Replace("{resultDetails}", "")
                        .Replace("{errors}", errorsHtml);

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(finalResultPage);
                });

                endpoints.MapGet("/question2", async context =>
                {
                    Console.WriteLine("Question 2");
                    var urls = BestWebsites.MyNotSensitivePageUrls();
                    Console.WriteLine("urls: " + urls.Count + " urls");
                    var ages = await Calculation.GetPageAges(urls);
                    var averageAge = Calculation.CalculateAverageAge(ages);
                    var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);
                    var resultsHtml = HtmlGenerator.GenerateResultsHtml(averageAge, standardDeviation);
                    Console.WriteLine("resultsHtml: " + resultsHtml);
                    var resultPageTemplatePath = Path.Combine(env.WebRootPath, "result-page-template.html");
                    var resultPageTemplate = await File.ReadAllTextAsync(resultPageTemplatePath);
                    var finalResultPage = resultPageTemplate.Replace("{pageTitle}", "Question 2")
                        .Replace("{resultSummary}", resultsHtml)
                        .Replace("{resultDetails}", "")
                        .Replace("{errors}", "");

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(finalResultPage);
                });

                endpoints.MapGet("/question3", async context =>
                {
                    var sb = new StringBuilder();

                    sb.Append("<h1>Test Scenarios</h1>");

                    sb.Append("<h2>Web servers example</h2>");
                    await ExecuteScenario(TestScenarios.Scenario1, sb);

                    sb.Append("<h2>News journals example</h2>");
                    await ExecuteScenario(TestScenarios.Scenario2, sb);

                    sb.Append("<h2>Big companies example</h2>");
                    await ExecuteScenario(TestScenarios.Scenario3, sb);

                    sb.Append("<h2>Useless websites example</h2>");
                    await ExecuteScenario(TestScenarios.Scenario4, sb);

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(sb.ToString());
                });
            });
            
        }

        private static async Task ExecuteScenario(Func<Task> scenario, StringBuilder sb)
        {
            try
            {
                await scenario.Invoke();
                sb.Append("<p>Scenario executed successfully</p>");
            }
            catch (Exception ex)
            {
                sb.Append($"<p>Scenario failed with exception: {ex.Message}</p>");
            }
        }
    }
}