namespace HeaderHTTPproject;

public static class Calculation
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(5) };

    /**
     * @param urls The list of URLs to check.
     * @return A dictionary of server names and the number of times they appear in the list of URLs.
     */
    public static async Task<Dictionary<string, int>> GetServerCounts(List<string> urls, List<string> errors)
    {
        var serverCounts = new Dictionary<string, int>();
        foreach (var url in urls)
        {
            var serverName = await GetServerName(url, errors);
            if (serverCounts.ContainsKey(serverName)) serverCounts[serverName]++;
            else serverCounts.Add(serverName, 1);
        }

        return serverCounts;
    }

    private static async Task<string> GetServerName(string url, List<string> errors)
    {
        var response = await HttpClient.GetAsync(url);
        var serverName = response.Headers.Server?.ToString();
        if (serverName == null)
        {
            errors.Add($"No server header for {url}");
            return "Unknown";
        }

        return serverName;
    }

    /**
     * Gets the age of a page. The age is the difference between the current time and the date of the page.
     * 
     * @param url The URL of the page.
     * @return The age of the page in seconds I guess
     */
    public static async Task<double> GetPageAge(string url, List<string> errors)
    {
        var response = await HttpClient.GetAsync(url);
        var ageValue = response.Headers.Age.HasValue
            ? (DateTime.UtcNow - response.Headers.Age.Value).Second
            : (int?)null;
        if (!ageValue.HasValue) errors.Add($"No age header for {url}");
        return ageValue ?? 0;
    }

    public static async Task<List<HeaderData>> GetImportantHeaderDataOfPages(List<string> urls, List<string> errors)
    {
        var results = new List<HeaderData>();
        foreach (var url in urls) results.Add(await GetImportantHeaderData(url, errors));
        return results;
    }

    private static async Task<HeaderData> GetImportantHeaderData(string url, List<string> errors)
    {
        var response = await HttpClient.GetAsync(url);

        var contentLength = response.Content.Headers.ContentLength ?? 0;
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "Unknown";
        var ageValue = response.Headers.Age?.TotalSeconds ?? 0.0;

        var lastModified = DateTimeOffset.MinValue;
        if (response.Content.Headers.LastModified != null)
        {
            if (DateTimeOffset.TryParse(response.Content.Headers.LastModified.GetValueOrDefault().ToString(),
                    out var lastModifiedResult))
                lastModified = lastModifiedResult;
            else
                errors.Add($"Invalid Last-Modified header for {url}");
        }
        else
        {
            errors.Add($"No Last-Modified header for {url}");
        }

        if (contentLength == 0L) errors.Add($"No content length header for {url}");
        if (contentType == "Unknown") errors.Add($"No content type header for {url}");
        if (ageValue == 0.0) errors.Add($"No age header for {url}");

        return new HeaderData(url, ageValue, contentLength, contentType, lastModified);
    }


    /**
     * Calculates the average age of a list of ages.
     * 
     * @param ages The list of ages.
     * @return The average age.
     */
    public static double CalculateAverageAge(List<double> ages)
    {
        if (ages.Count == 0) return 0;
        return ages.Average();
    }

    /**
     * Calculates the standard deviation of a list of ages.
     * Ecart-type = racine carrée de la somme des carrés des écarts à la moyenne divisé par le nombre d'éléments.
     *  
     * @param ages The list of ages.
     * @param averageAge The average age of the list.
     * 
     * @return The standard deviation.
     */
    public static double CalculateStandardDeviation(IReadOnlyCollection<double> ages, double averageAge)
    {
        if (ages.Count == 0) return 0;
        if (averageAge == 0) return 0;
        var sum = ages.Sum(age => Math.Pow(age - averageAge, 2));
        return Math.Sqrt(sum / ages.Count);
    }

    /**
     * Formats a number of bytes into a human-readable string.
     */
    public static string FormatBytes(long bytes)
    {
        string[] units = { "bytes", "KB", "MB", "GB" };
        double size = bytes;
        var unitIndex = 0;

        while (size > 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:0.##} {units[unitIndex]}";
    }
}