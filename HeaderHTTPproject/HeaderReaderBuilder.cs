using System.Text;

namespace HeaderHTTPproject;

public static class HeaderReaderBuilder
{
    public static void AddAges(List<HeaderData> headerData, StringBuilder sb)
    {
        var ages = headerData.Select(x => x.AgeValue).ToList();
        var averageAge = Calculation.CalculateAverageAge(ages);
        var standardDeviation = Calculation.CalculateStandardDeviation(ages, averageAge);

        sb.Append("<p>Average age: ")
            .Append(averageAge)
            .Append(" seconds</p>")
            .AppendLine();

        sb.Append("<p>Standard deviation: ")
            .Append(standardDeviation)
            .Append(" seconds</p>")
            .AppendLine();
    }

    public static void AddTotalContentLength(List<HeaderData> headerData, StringBuilder sb)
    {
        var totalContentLength = headerData.Sum(x => x.ContentLength);
        var averageContentLength = totalContentLength / headerData.Count;

        sb.Append("<p>Total content length: ")
            .Append(Calculation.FormatBytes(totalContentLength))
            .Append("</p>")
            .AppendLine();

        sb.Append("<p>Average content length: ")
            .Append(Calculation.FormatBytes(averageContentLength))
            .Append("</p>")
            .AppendLine();
    }

    public static void AddContentType(List<HeaderData> headerData, StringBuilder sb)
    {
        var contentTypeCounts = headerData.GroupBy(x => x.ContentType)
            .ToDictionary(x => x.Key, x => x.Count());

        foreach (var (contentType, count) in contentTypeCounts)
            sb.Append("<p>")
                .Append(contentType)
                .Append(": ")
                .Append(count)
                .Append("</p>")
                .AppendLine();
    }

    public static void AddLastModificationDate(List<string> errors, List<HeaderData> headerData, StringBuilder sb)
    {
        if (headerData.All(x => x.LastModification == DateTimeOffset.MinValue))
        {
            sb.Append("<p>No Last-Modified header found</p>");
            return;
        }

        sb.Append("<h4>Last modification date</h4>");

        foreach (var (url, lastModified) in headerData.Select(x => (x.Url, x.LastModification)))
        {
            if (lastModified == DateTimeOffset.MinValue) continue;
            sb.Append("<p>").Append(url).Append(": ").Append(lastModified).Append("</p>").AppendLine();
        }
    }
}