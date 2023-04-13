namespace HeaderHTTPproject
{
    public class Question2
    {
        public static async Task Run(IEnumerable<string> urls)
        {
            var headerData = await Startup.GetPageAgesAsync2(urls);

            var ages = headerData.Select(x => x.age).ToList();
            var averageAge = Startup.CalculateAverageAge(ages);
            var standardDeviation = Startup.CalculateStandardDeviation(ages, averageAge);

            Console.WriteLine($"\nAverage age: {averageAge} seconds");
            Console.WriteLine($"Standard deviation: {standardDeviation} seconds");

        }
    }
}