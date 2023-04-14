namespace HeaderHTTPproject;

public class Question1
{
    public static async Task Run(List<string> serverUrls)
    {
        var errors = new List<string>();
        var serverCounts = await Calculation.GetServerCounts(serverUrls, errors);

        Console.WriteLine("Server statistics:");
        foreach (var (serverName, count) in serverCounts) Console.WriteLine($"{serverName}: {count}");

        if (errors.Count > 0)
        {
            Console.WriteLine("\nErrors:");
            foreach (var error in errors) Console.WriteLine(error);
        }
    }
}