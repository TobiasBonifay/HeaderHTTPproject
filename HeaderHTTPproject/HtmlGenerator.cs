using System.Text;
using Microsoft.AspNetCore.Http;

namespace HeaderHTTPproject;

/**
 * This class is used to generate the HTML for the results page.
 */
public class HtmlGenerator
{
    public static async Task<List<string>> GetUrlsFromForm(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();
        return form.Keys.SelectMany(key => form[key].ToString().Split(',')).ToList();
    }

    public static string GenerateResultsHtml(Dictionary<string, int> serverCounts, int totalCount)
    {
        var serverCountsHtml = new StringBuilder();
        foreach (var (serverName, count) in serverCounts)
        {
            var percentage = count / (double)totalCount * 100;
            serverCountsHtml.Append($"<tr><td>{serverName}</td>" +
                                    $"<td>{count}</td>" +
                                    $"<td>{percentage:N2}%</td></tr>");
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
}