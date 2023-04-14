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
                    var errorAccumulator = new List<string>();
                    switch (scenario)
                    {
                        case "question1":
                            var urls1 = BestWebsites.MyNotSensitivePageUrls();
                            var serverCounts = await Calculation.GetServerCounts(urls1, errorAccumulator);
                            resultsHtml = HtmlGenerator.GenerateResultsHtml(serverCounts, urls1.Count);
                            break;

                        case "question2":
                            var urls2 = BestWebsites.DifferentPagesOfTheSameWebsites();
                            var ages = new List<double>();
                            foreach (var url in urls2)
                            {
                                var age = await Calculation.GetPageAge(url, errorAccumulator);
                                if (age.HasValue) ages.Add(age.Value);
                                else errorAccumulator.Add("No age found for " + url + ",");
                            }
                            var averageAge = Calculation.CalculateAverageAge(ages);
                            var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);
                            resultsHtml = HtmlGenerator.GenerateResultsHtml(averageAge, standardDeviation);
                            break;

                        case "question3":
                            var sb = new StringBuilder();
                            sb.Append("<h2>Test Scenarios</h2>");
                            sb.Append(await TestScenarios.Scenario1(errorAccumulator));
                            sb.Append(await TestScenarios.Scenario2(errorAccumulator));
                            sb.Append(await TestScenarios.Scenario3(errorAccumulator));
                            resultsHtml = sb.ToString();
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
                        .Replace("{errors}", HtmlGenerator.GenerateErrorsHtml(errorAccumulator));
                    await context.Response.WriteAsync(finalResultsHtml);
                });

                
                endpoints.MapGet("/question1", async context =>
                {
                    var urls = BestWebsites.MyNotSensitivePageUrls();
                    var errors = new List<string>();
                    var serverCounts = await Calculation.GetServerCounts(urls, errors);

                    var resultPageTemplatePath = Path.Combine(env.WebRootPath, "result-page-template.html");
                    var resultPageTemplate = await File.ReadAllTextAsync(resultPageTemplatePath);

                    var finalResultPage = resultPageTemplate.Replace("{pageTitle}", "Question 1")
                        .Replace("{resultSummary}", HtmlGenerator.GenerateResultsHtml(serverCounts, urls.Count))
                        .Replace("{resultDetails}", "")
                        .Replace("{errors}", HtmlGenerator.GenerateErrorsHtml(errors));

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
                    var sb = new StringBuilder();
                    var errors = new List<string>();
                    sb.Append("<h2>Test Scenarios</h2>");
                    sb.Append(await TestScenarios.Scenario1(errors));
                    sb.Append(await TestScenarios.Scenario2(errors));
                    sb.Append(await TestScenarios.Scenario3(errors));
                    var resultPageTemplatePath = Path.Combine(env.WebRootPath, "result-page-template.html");
                    var resultPageTemplate = await File.ReadAllTextAsync(resultPageTemplatePath);
                    var finalResultPage = resultPageTemplate.Replace("{pageTitle}", "Question 3")
                        .Replace("{resultSummary}", "")
                        .Replace("{resultDetails}", sb.ToString())
                        .Replace("{errors}", HtmlGenerator.GenerateErrorsHtml(errors));

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(sb.ToString());
                });
            });
            
        }
    }
}