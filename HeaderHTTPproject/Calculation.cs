using System.Text;

namespace HeaderHTTPproject;

public class Calculation
{
    private static HttpClient HttpClient { get; } = new() { Timeout = TimeSpan.FromSeconds(5) };


    /**
     * @param urls The list of URLs to check.
     * @return A dictionary of server names and the number of times they appear in the list of URLs.
     */
    public static async Task<Dictionary<string, int>> GetServerCounts(List<string> urls, List<string> errors)
    {
        var serverCounts = new Dictionary<string, int>();

        foreach (var url in urls)
            try
            {
                var response = await HttpClient.GetAsync(url);
                var server = response.Headers.Server.ToString();
                if (string.IsNullOrWhiteSpace(server))
                {
                    server = "Unknown";
                    errors.Add($"No server header for {url}");
                }
                if (!serverCounts.ContainsKey(server)) serverCounts[server] = 0;
                serverCounts[server]++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
                errors.Add($"Error fetching headers for {url}: {ex.Message}");
            }

        return serverCounts;
    }


    /**
     * Gets the age of a page. The age is the difference between the current time and the date of the page.
     * 
     * @param url The URL of the page.
     * @return The age of the page in seconds I guess
     */
    public static async Task<double?> GetPageAge(string url, List<string> errors)
    {
        try
        {
            var response = await HttpClient.GetAsync(url);
            Console.WriteLine($"Age of {url}: {response.Headers.Age}");
            if (!response.Headers.Age.HasValue)
            {
                Console.WriteLine($"No age header for {url}");
                errors.Add($"No age header for {url}");
                return null;
            }
            var age = (DateTime.UtcNow - response.Headers.Age.Value).Second;
            return age;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
            errors.Add($"Error fetching headers for {url}: {ex.Message}");
            return null;
        }
    }

    public static async Task<List<double>> GetPageAges(List<string> urls, List<string> errors)
    {
        var ages = new List<double>();

        foreach (var url in urls)
            try
            {
                var age = await GetPageAge(url, errors);
                if (!age.HasValue) continue;
                ages.Add(age.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
                errors.Add($"Error fetching headers for {url}: {ex.Message}");
            }

        return ages;
    }

    /**
     * Gets the age of a page. The age is the difference between the current time and the date of the page.
     * 
     * @param url The URL of the page.
     * @return The age of the page in seconds I guess
     */
    public static async Task<List<HeaderData>> GetImportantHeaderDataOfPages(List<string> urls, List<string> errors)
    {
        var results = new List<HeaderData>();
        using var httpClient = new HttpClient();

        foreach (var url in urls)
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                var contentLength = response.Content.Headers.ContentLength ?? 0;
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "Unknown";
                var ageValue = response.Headers.Age.HasValue ? (DateTime.UtcNow - response.Headers.Age.Value).Second : 0;
                var lastModification = response.Content.Headers.LastModified?.UtcDateTime ?? DateTimeOffset.MinValue;

                results.Add(new HeaderData
                {
                    Url = url,
                    AgeValue = ageValue,
                    ContentLength = contentLength,
                    ContentType = contentType,
                    LastModification = lastModification
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
                errors.Add($"Error fetching headers for {url}: {ex.Message}");
            }
        }

        return results;
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
        return ages.Sum() / ages.Count;
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