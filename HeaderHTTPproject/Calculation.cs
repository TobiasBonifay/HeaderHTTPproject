﻿namespace HeaderHTTPproject
{
    public class Calculation
    {
        private static HttpClient HttpClient { get; } = new() { Timeout = TimeSpan.FromSeconds(5) };
        

        /**
         * @param urls The list of URLs to check.
         * @return A dictionary of server names and the number of times they appear in the list of URLs.
         */
        public static async Task<(Dictionary<string, int>, List<string>)> GetServerCounts(IEnumerable<string> urls)
        {
            var serverCounts = new Dictionary<string, int>();
            var errors = new List<string>();

            foreach (var url in urls)
            {
                try
                {
                    var response = await HttpClient.GetAsync(url);
                    var server = response.Headers.Server.ToString();
                    if (string.IsNullOrWhiteSpace(server)) server = "Unknown";
                    if (!serverCounts.ContainsKey(server)) serverCounts[server] = 0;
                    serverCounts[server]++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
                    errors.Add($"Error fetching headers for {url}: {ex.Message}");
                }
            }

            return (serverCounts, errors);
        }
        

        /**
         * Gets the age of a page. The age is the difference between the current time and the date of the page.
         *
         * @param url The URL of the page.
         * @return The age of the page in seconds I guess
         */
        public static async Task<double?> GetPageAge(string url)
        {
            try
            {
                var response = await HttpClient.GetAsync(url);
                if (!response.Headers.Age.HasValue) return null;
                var age = (DateTime.UtcNow - response.Headers.Age.Value).Second;
                return age;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
                return null;
            }
        }
        
        /**
         * Gets the age of a page. The age is the difference between the current time and the date of the page.
         * 
         * @param url The URL of the page.
         * @return The age of the page in seconds I guess
         */
        public static async Task<List<(double age, long contentLength, string contentType)>> GetImportantHeaderDataOfPages(IEnumerable<string> urls)
        {
            var results = new List<(double age, long contentLength, string contentType)>();

            foreach (var url in urls)
            {
                try
                {
                    var age = await GetPageAge(url);
                    if (!age.HasValue) continue;

                    var response = await HttpClient.GetAsync(url);
                    var contentLength = response.Content.Headers.ContentLength ?? 0;
                    var contentType = response.Content.Headers.ContentType?.MediaType ?? "Unknown";

                    results.Add((age.Value, contentLength, contentType));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
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
}