namespace HeaderHTTPproject;

public static class Question1
{
    public static async Task<string> Run(List<string> errorAccumulator)
    {
        var urls = BestWebsites.MyNotSensitivePageUrls();
        return await Run(urls, errorAccumulator);
    }

    public static async Task<string> Run(List<string> urls, List<string> errorAccumulator)
    {
        var serverCounts = await Calculation.GetServerCounts(urls, errorAccumulator);
        return HtmlGenerator.GenerateResultsHtml(serverCounts, urls.Count);
    }
}