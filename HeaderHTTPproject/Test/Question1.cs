namespace HeaderHTTPproject
{
    public class Question1
    {
        public static async Task Run(IEnumerable<string> serverUrls)
        {
            var (serverCounts, errors) = await Calculation.GetServerCounts(serverUrls);

            Console.WriteLine("Server statistics:");
            foreach (var (serverName, count) in serverCounts) Console.WriteLine($"{serverName}: {count}");

            if (errors.Count > 0)
            {
                Console.WriteLine("\nErrors:");
                foreach (var error in errors) Console.WriteLine(error);
            }
        }
    }
}