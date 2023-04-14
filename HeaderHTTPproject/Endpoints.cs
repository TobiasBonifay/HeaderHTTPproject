using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;

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
                    // var urls = await HtmlGenerator.GetUrlsFromForm(context);
                    var scenario = context.Request.Form["scenario"];

                    string? resultsHtml;
                    string? errorsHtml = null;
                    switch (scenario)
                    {
                        case "question1":
                            var errors1 = new List<string>();
                            var urls1 = BestWebsites.MyNotSensitivePageUrls();
                            var serverCounts = await Calculation.GetServerCounts(urls1, errors1);
                            resultsHtml = HtmlGenerator.GenerateResultsHtml(serverCounts, urls1.Count);
                            errorsHtml = HtmlGenerator.GenerateErrorsHtml(errors1);
                            break;

                        case "question2":
                            var errors2 = new List<string>();
                            var urls2 = BestWebsites.DifferentPagesOfTheSameWebsites();
                            var ages = new List<double>();
                            foreach (var url in urls2)
                            {
                                var age = await Calculation.GetPageAge(url, errors2);
                                if (age.HasValue) ages.Add(age.Value);
                                else errors2.Add("No age found for " + url + ",");
                            }
                            var averageAge = Calculation.CalculateAverageAge(ages);
                            var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);
                            resultsHtml = HtmlGenerator.GenerateResultsHtml(averageAge, standardDeviation);
                            errorsHtml = HtmlGenerator.GenerateErrorsHtml(errors2);
                            break;

                        case "question3":
                            var sb = new StringBuilder();
                            var errors3 = new List<string>();
                            sb.Append("<h2>Test Scenarios</h2>");
                            var serverCounts1 = await Calculation.GetServerCounts(TestScenarios.Scenario1, errors3);
                            sb.Append(serverCounts1);
                            var serverCounts2 = await Calculation.GetServerCounts(TestScenarios.Scenario2, errors3);
                            sb.Append(serverCounts2);
                            var serverCounts3 = await Calculation.GetServerCounts(TestScenarios.Scenario3, errors3);
                            sb.Append(serverCounts3);
                            resultsHtml = sb.ToString();
                            errorsHtml = HtmlGenerator.GenerateErrorsHtml(errors3);
                            break;

                        default:
                            resultsHtml = "<p>Invalid scenario selected</p>";
                            break;
                    }

                    var resultsHtmlTemplatePath = Path.Combine(env.WebRootPath, "result-page-template.html");
                    var resultsHtmlTemplate = await File.ReadAllTextAsync(resultsHtmlTemplatePath);

                    var finalResultsHtml = resultsHtmlTemplate.Replace("{pageTitle}", "Results")
                        .Replace("{resultSummary}", "")
                        .Replace("{resultDetails}", resultsHtml)
                        .Replace("{errors}", errorsHtml);
                    await context.Response.WriteAsync(finalResultsHtml);
                });

                
                endpoints.MapGet("/question1", async context =>
                {
                    var urls = BestWebsites.MyNotSensitivePageUrls();
                    var errors = new List<string>();
                    var serverCounts = await Calculation.GetServerCounts(urls, errors);
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
                    var urls = BestWebsites.DifferentPagesOfTheSameWebsites();
                    Console.WriteLine("urls: " + urls.Count + " urls");
                    var errors2 = new List<string>();
                    var ages = await Calculation.GetPageAges(urls, errors2);
                    var averageAge = Calculation.CalculateAverageAge(ages);
                    var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);
                    var resultsHtml = HtmlGenerator.GenerateResultsHtml(averageAge, standardDeviation);
                    Console.WriteLine("resultsHtml: " + resultsHtml);
                    var resultPageTemplatePath = Path.Combine(env.WebRootPath, "result-page-template.html");
                    var resultPageTemplate = await File.ReadAllTextAsync(resultPageTemplatePath);
                    var finalResultPage = resultPageTemplate.Replace("{pageTitle}", "Question 2")
                        .Replace("{resultSummary}", resultsHtml)
                        .Replace("{resultDetails}", "")
                        .Replace("{errors}", HtmlGenerator.GenerateErrorsHtml(errors2));

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(finalResultPage);
                });

                endpoints.MapGet("/question3", async context =>
                {
                    /**
                    var sb = new StringBuilder();
                    sb.Append("<h2>Test Scenarios</h2>");
                    sb.Append(await ExecuteScenario(TestScenarios.Scenario1));
                    sb.Append(await ExecuteScenario(TestScenarios.Scenario2));
                    sb.Append(await ExecuteScenario(TestScenarios.Scenario3));
                    sb.Append(await ExecuteScenario(TestScenarios.Scenario4));

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(sb.ToString());
                    */
                });
            });
            
        }

        private static async Task<string> ExecuteScenario(Func<Task<string>> scenario)
        {
            var sb = new StringBuilder();
            try
            {
                var result = await scenario.Invoke();
                sb.Append(result);
            }
            catch (Exception ex)
            {
                sb.Append($"<p>Scenario failed with exception: {ex.Message}</p>");
            }
            return sb.ToString();
        }
    }
}