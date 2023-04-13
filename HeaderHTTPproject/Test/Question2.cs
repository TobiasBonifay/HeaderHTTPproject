namespace HeaderHTTPproject
{
    public class Question2
    {
        public static async Task Run(IEnumerable<string> urls)
        {
            var headerData = await Calculation.GetImportantHeaderDataOfPages(urls);

            var ages = headerData.Select(x => x.age).ToList();
            var averageAge = Calculation.CalculateAverageAge(ages);
            var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);

            Console.WriteLine($"\nAverage age: {averageAge} seconds");
            Console.WriteLine($"Standard deviation: {standardDeviation} seconds");

        }
    }
}