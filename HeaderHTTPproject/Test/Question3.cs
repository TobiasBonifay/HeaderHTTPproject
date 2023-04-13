using System.Text;
using Microsoft.Extensions.Primitives;

namespace HeaderHTTPproject;

public class TestScenarios
{
    public static async Task<string> Scenario1()
    {
        return await RunTestScenario("Web servers example", BestWebsites.WebServers(), new StringBuilder());
    }

    public static async Task<string> Scenario2()
    {
        return await RunTestScenario("News journals example", BestWebsites.NewsWebsite(), new StringBuilder());
    }

    public static async Task<string> Scenario3()
    {
        return await RunTestScenario("Big companies example", BestWebsites.BigCompanies(), new StringBuilder());
    }

    public static async Task<string> Scenario4()
    {
        return await RunTestScenario("Useless websites example", BestWebsites.UselessWebsites(), new StringBuilder());
    }

    private static async Task<string> RunTestScenario(string scenarioName, List<string> urls, StringBuilder sb)
    {
        sb.Append("<h3>").Append(scenarioName).Append("</h3>");
        var headerData = await Calculation.GetImportantHeaderDataOfPages(urls);

        var ages = headerData.Select(x => x.age).ToList();
        var averageAge = Calculation.CalculateAverageAge(ages);
        var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);
        sb.Append("<p>Average age: ").Append(averageAge).Append(" seconds</p>").AppendLine();
        sb.Append("<p>Standard deviation: ").Append(standardDeviation).Append(" seconds</p>").AppendLine();

        var totalContentLength = headerData.Sum(x => x.contentLength);
        var averageContentLength = totalContentLength / headerData.Count;
        sb.Append("<p>Total content length: ").Append(Calculation.FormatBytes(totalContentLength)).Append("</p>").AppendLine();
        sb.Append("<p>Average content length: ").Append(Calculation.FormatBytes(averageContentLength)).Append("</p>").AppendLine();

        var contentTypeCounts = headerData.GroupBy(x => x.contentType)
            .ToDictionary(x => x.Key, x => x.Count());
        foreach (var (contentType, count) in contentTypeCounts) sb.Append("<p>").Append(contentType).Append(": ").Append(count).Append("</p>").AppendLine();
        return sb.ToString();
    }
}