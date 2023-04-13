using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeaderHTTPproject
{
    public class TestScenarios
    {
        public static async Task Scenario1()
        {
            var urls = new List<string>
            {
                "http://httpd.apache.org/",
                "https://www.iis.net/",
                "https://www.nginx.com/",
                "https://www.lighttpd.net/",
                "https://gunicorn.org/",
            };

            await RunTestScenario("Web servers example", urls);
        }

        public static async Task Scenario2()
        {
            var urls = new List<string>
            {
                "https://www.nytimes.com/",
                "https://www.bbc.com/",
                "https://edition.cnn.com/",
                "https://www.theguardian.com/",
                "https://www.latimes.com/",
            };

            await RunTestScenario("News journals example", urls);
        }

        public static async Task Scenario3()
        {
            var urls = new List<string>
            {
                "https://www.google.com/",
                "https://www.youtube.com/",
                "https://www.facebook.com/",
                "https://www.yahoo.com/",
                "https://www.amazon.com/",
            };

            await RunTestScenario("Big companies example", urls);
        }
        
        public static async Task Scenario4()
        {
            var urls = new List<string>
            {
                "http://www.koalastothemax.com/",
                "http://www.therestartpage.com/",
                "https://scatter.wordpress.com/2010/05/30/the-shortest-possible-game-of-monopoly-21-seconds/",
                "https://paint.toys/"
            };

            await RunTestScenario("Useless websites example", urls);
        }

        private static async Task RunTestScenario(string scenarioName, List<string> urls)
        {
            Console.WriteLine($"\n{scenarioName}");
            var (serverCounts, errors) = await Startup.GetServerCountsAsync(urls);
            // foreach (var (serverName, count) in serverCounts) Console.WriteLine($"{serverName}: {count}");
            var headerData = await Startup.GetPageAgesAsync2(urls);

            var ages = headerData.Select(x => x.age).ToList();
            var averageAge = Startup.CalculateAverageAge(ages);
            var standardDeviation = Startup.CalculateStandardDeviation(ages, averageAge);
            Console.WriteLine($"Average age: {averageAge} seconds");
            Console.WriteLine($"Standard deviation: {standardDeviation} seconds");

            var totalContentLength = headerData.Sum(x => x.contentLength);
            var averageContentLength = totalContentLength / headerData.Count;
            Console.WriteLine($"Total content length: {Startup.FormatBytes(totalContentLength)}");
            Console.WriteLine($"Average content length: {Startup.FormatBytes(averageContentLength)}");

            var contentTypeCounts = headerData.GroupBy(x => x.contentType)
                .ToDictionary(x => x.Key, x => x.Count());
            foreach (var (contentType, count) in contentTypeCounts) Console.WriteLine($"{contentType}: {count}");
        }
    }
}
