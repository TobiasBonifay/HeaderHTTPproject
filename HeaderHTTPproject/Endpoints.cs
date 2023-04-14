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
                            resultsHtml = Question1.Run(errorAccumulator).Result;
                            break;

                        case "question2":
                            resultsHtml = Question2.Run(errorAccumulator).Result;
                            break;

                        case "question3":
                            resultsHtml = Question3.Run(errorAccumulator).Result;
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
                    var errors = new List<string>();
                    
                    var resultsHtml = Question1.Run(errors).Result;

                    var resultPageTemplatePath = Path.Combine(env.WebRootPath, "result-page-template.html");
                    var resultPageTemplate = await File.ReadAllTextAsync(resultPageTemplatePath);

                    var finalResultPage = resultPageTemplate.Replace("{pageTitle}", "Question 1")
                        .Replace("{resultSummary}", resultsHtml)
                        .Replace("{resultDetails}", "")
                        .Replace("{errors}", HtmlGenerator.GenerateErrorsHtml(errors));

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(finalResultPage);
                });

                endpoints.MapGet("/question2", async context =>
                {
                    var errors2 = new List<string>();
                    
                    var resultsHtml = await Question2.Run(errors2);

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
                    var errors = new List<string>();

                    var resultsHtml = await Question3.Run(errors);
                    
                    var resultPageTemplatePath = Path.Combine(env.WebRootPath, "result-page-template.html");
                    var resultPageTemplate = await File.ReadAllTextAsync(resultPageTemplatePath);
                    var finalResultPage = resultPageTemplate.Replace("{pageTitle}", "Question 3")
                        .Replace("{resultSummary}", "")
                        .Replace("{resultDetails}", resultsHtml)
                        .Replace("{errors}", HtmlGenerator.GenerateErrorsHtml(errors));

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(finalResultPage);
                });
            });
            
        }
    }
}