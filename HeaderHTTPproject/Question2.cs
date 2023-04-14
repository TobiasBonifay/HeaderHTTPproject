namespace HeaderHTTPproject;

public static class Question2
{
    public static async Task<string> Run(List<string> errorAccumulator)
    {
        var urls2 = BestWebsites.DifferentPagesOfTheSameWebsites();
        return await Run(urls2, errorAccumulator);
    }

    public static async Task<string> Run(List<string> urls, List<string> errorAccumulator)
    {
        var ages = new List<double>();
        foreach (var url in urls) ages.Add(await Calculation.GetPageAge(url, errorAccumulator));

        var averageAge = Calculation.CalculateAverageAge(ages);
        var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);
        return HtmlGenerator.GenerateResultsHtml(averageAge, standardDeviation);
    }
}