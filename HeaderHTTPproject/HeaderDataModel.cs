namespace HeaderHTTPproject;

public class HeaderData
{
    public HeaderData(string url, double ageValue, long contentLength, string contentType,
        DateTimeOffset lastModification)
    {
        Url = url;
        AgeValue = ageValue;
        ContentLength = contentLength;
        ContentType = contentType;
        LastModification = lastModification;
    }

    public string Url { get; set; }
    public double AgeValue { get; set; }
    public long ContentLength { get; set; }
    public string ContentType { get; set; }
    public DateTimeOffset LastModification { get; set; }
}