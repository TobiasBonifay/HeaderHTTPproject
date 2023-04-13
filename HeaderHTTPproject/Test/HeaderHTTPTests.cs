using Xunit;
using HeaderHTTPproject;
using System.Collections.Generic;
using System.Reflection;

namespace HeaderHTTPproject.Tests
{
    public class HeaderHTTPTests
    {
        [Fact]
        public void GenerateResultsHtmlTest()
        {
            var serverCounts = new Dictionary<string, int>
            {
                { "Server 1", 2 },
                { "Server 2", 1 },
                { "Server 3", 3 },
            };
            int totalCount = 6;

            var methodInfo = typeof(Startup).GetMethod("GenerateResultsHtml", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(methodInfo);

            var resultsHtml = (string)methodInfo.Invoke(null, new object[] { serverCounts, totalCount })!;

            Assert.Contains("<tr><td>Server 1</td><td>2</td><td>33.33%</td></tr>", resultsHtml);
            Assert.Contains("<tr><td>Server 2</td><td>1</td><td>16.67%</td></tr>", resultsHtml);
            Assert.Contains("<tr><td>Server 3</td><td>3</td><td>50.00%</td></tr>", resultsHtml);
        }
    }
}
