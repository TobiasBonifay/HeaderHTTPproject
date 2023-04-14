using System.Text;
using Microsoft.Extensions.Primitives;

namespace HeaderHTTPproject;

public class Question3
{
    
    public static async Task<string> Run(List<string> errors)
    {
        var sb = new StringBuilder();
        sb.Append("<h2>Test Scenarios</h2>");
        sb.Append(await Scenario1(errors));
        sb.Append(await Scenario2(errors));
        sb.Append(await Scenario3(errors));
        sb.Append(await Scenario4(errors));
        return sb.ToString();
    }
    
    public static async Task<string?> Run(List<string> urls, List<string> errors)
    {
        return await RunTestScenario("", urls, errors);
    }

    
    private static async Task<string> Scenario1(List<string> errors)
    {
        return await RunTestScenario("Web servers example", BestWebsites.WebServers(), errors);
    }

    private static async Task<string> Scenario2(List<string> errors)
    {
        return await RunTestScenario("News journals example", BestWebsites.NewsWebsite(), errors);
    }

    private static async Task<string> Scenario3(List<string> errors)
    {
        return await RunTestScenario("Big companies example", BestWebsites.BigCompanies(), errors);
    }

    private static async Task<string> Scenario4(List<string> errors)
    {
        return await RunTestScenario("Useless websites example", BestWebsites.UselessWebsites(), errors);
    }

    private static async Task<string> RunTestScenario(string scenarioName, List<string> urls, List<string> errors)
    {
        var sb = new StringBuilder();
        sb.Append("<h3>").Append(scenarioName).Append("</h3>");
        
        var headerData = await Calculation.GetImportantHeaderDataOfPages(urls, errors);

        HeaderReaderBuilder.AddAges(headerData, sb);
        HeaderReaderBuilder.AddTotalContentLength(headerData, sb);
        HeaderReaderBuilder.AddContentType(headerData, sb);
        HeaderReaderBuilder.AddLastModificationDate(errors, headerData, sb);

        return sb.ToString();
    }
}