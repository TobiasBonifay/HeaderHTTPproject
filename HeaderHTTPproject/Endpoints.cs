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
                    var scenario = context.Request.Form["scenario"];

                    string resultsHtml;
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
                        
                        case "yourUrl":
                            var urls = await HtmlGenerator.GetUrlsFromForm(context);
                            resultsHtml = await Question1.Run(urls, errorAccumulator);
                            // resultsHtml += await Question2.Run(urls, errorAccumulator);
                            resultsHtml += await Question3.Run(urls, errorAccumulator);
                            break;

                        default:
                            resultsHtml = "<p>Invalid scenario selected</p>";
                            break;
                    }

                    await HtmlGenerator.GenerateResultPage(context, env, "Results", resultsHtml, errorAccumulator);
                });

                
                endpoints.MapGet("/question1", async context =>
                {
                    var errors = new List<string>();
                    var resultsHtml = Question1.Run(errors).Result;
                    await HtmlGenerator.GenerateResultPage(context, env, "Question 1", resultsHtml, errors);
                });

                endpoints.MapGet("/question2", async context =>
                {
                    var errors = new List<string>();
                    var resultsHtml = await Question2.Run(errors);
                    await HtmlGenerator.GenerateResultPage(context, env, "Question 2", resultsHtml, errors);
                });

                endpoints.MapGet("/question3", async context =>
                {
                    var errors = new List<string>();
                    var resultsHtml = await Question3.Run(errors);
                    await HtmlGenerator.GenerateResultPage(context, env, "Question 3", resultsHtml, errors);
                });
            });
        }
    }
}
