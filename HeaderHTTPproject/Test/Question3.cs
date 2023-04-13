namespace HeaderHTTPproject;

public class TestScenarios
{
    public static async Task Scenario1()
    {
        await RunTestScenario("Web servers example", BestWebsites.WebServers());
    }

    public static async Task Scenario2()
    {
        await RunTestScenario("News journals example", BestWebsites.NewsWebsite());
    }

    public static async Task Scenario3()
    {
        await RunTestScenario("Big companies example", BestWebsites.BigCompanies());
    }

    public static async Task Scenario4()
    {
        await RunTestScenario("Useless websites example", BestWebsites.UselessWebsites());
    }

    private static async Task RunTestScenario(string scenarioName, List<string> urls)
    {
        Console.WriteLine($"\n{scenarioName}");
        // var (serverCounts, errors) = await Calculation.GetServerCounts(urls);
        // foreach (var (serverName, count) in serverCounts) Console.WriteLine($"{serverName}: {count}");
        var headerData = await Calculation.GetImportantHeaderDataOfPages(urls);

        var ages = headerData.Select(x => x.age).ToList();
        var averageAge = Calculation.CalculateAverageAge(ages);
        var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);
        Console.WriteLine($"Average age: {averageAge} seconds");
        Console.WriteLine($"Standard deviation: {standardDeviation} seconds");

        var totalContentLength = headerData.Sum(x => x.contentLength);
        var averageContentLength = totalContentLength / headerData.Count;
        Console.WriteLine($"Total content length: {Calculation.FormatBytes(totalContentLength)}");
        Console.WriteLine($"Average content length: {Calculation.FormatBytes(averageContentLength)}");

        var contentTypeCounts = headerData.GroupBy(x => x.contentType)
            .ToDictionary(x => x.Key, x => x.Count());
        foreach (var (contentType, count) in contentTypeCounts) Console.WriteLine($"{contentType}: {count}");
    }
}