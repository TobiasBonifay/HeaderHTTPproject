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
    }
}
