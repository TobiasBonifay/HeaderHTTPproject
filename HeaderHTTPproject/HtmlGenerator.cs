using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HeaderHTTPproject;

/**
 * This class is used to generate the HTML for the results page.
 */
public static class HtmlGenerator
{
    public static async Task<List<string>> GetUrlsFromForm(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();
        var urls = form.Keys.SelectMany(key => form[key].ToString().Split(',')).ToList();
        urls.Remove("yourUrl");
        return urls;
    }

    public static string GenerateResultsHtml(Dictionary<string, int> serverCounts, int totalCount)
    {
        var serverCountsHtml =
            new StringBuilder("<h2>Results</h2><table><tr><th>Server</th><th>Count</th><th>Percentage</th></tr>");
        foreach (var (serverName, count) in serverCounts)
        {
            var percentage = count / (double)totalCount * 100;
            serverCountsHtml.Append($"<tr><td>{serverName}</td>" +
                                    $"<td>{count}</td>" +
                                    $"<td>{percentage:N2}%</td></tr>");
        }

        serverCountsHtml.Append("</table>");
        return serverCountsHtml.ToString();
    }

    private static string GenerateErrorsHtml(List<string> errors)
    {
        var errorsHtml = new StringBuilder();
        if (errors.Count == 0) return "";
        errorsHtml.Append("<h2>Errors</h2><ul>");
        foreach (var error in errors) errorsHtml.Append($"<li>{error}</li>");
        errorsHtml.Append("</ul>");
        return errorsHtml.ToString();
    }

    public static string GenerateResultsHtml(double averageAge, double standardDeviation)
    {
        return "<table><tr><th>Average Age</th><th>Standard Deviation</th></tr>" +
               $"<tr><td>{averageAge:N2} seconds</td>" +
               $"<td>{standardDeviation:N2} seconds</td></tr></table>";
    }

    public static async Task GenerateResultPage(HttpContext context, IWebHostEnvironment env, string pageTitle,
        string resultSummary, List<string> errors)
    {
        var resultPageTemplatePath = Path.Combine(env.WebRootPath, "result-page-template.html");
        var resultPageTemplate = await File.ReadAllTextAsync(resultPageTemplatePath);
        var finalResultPage = resultPageTemplate
            .Replace("{pageTitle}", pageTitle)
            .Replace("{resultSummary}", resultSummary)
            .Replace("{errors}", GenerateErrorsHtml(errors));

        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(finalResultPage);
    }
}