﻿using System.Text;

namespace HeaderHTTPproject;

public class HeaderReaderBuilder
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
        {
            sb.Append("<p>")
                .Append(contentType)
                .Append(": ")
                .Append(count)
                .Append("</p>")
                .AppendLine();
        }
    }

    public static void AddLastModificationDate(List<string> errors, List<HeaderData> headerData, StringBuilder sb)
    {
        foreach (var (url, lastModified) in headerData.Select(x => (x.Url, x.LastModification)))
        {
            sb.Append("<p>")
                .Append(url)
                .Append(": ");
            
            if (lastModified != DateTimeOffset.MinValue) sb.Append(lastModified);
            else errors.Add($"Error fetching Last modification date for {url}: No Last-Modified header");
            
            sb.Append("</p>")
                .AppendLine();
        }
    }
}