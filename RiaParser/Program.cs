using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiaParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var frequencies = new Dictionary<string, int>();
            var newsParser = new NewsParser();
            var filter = new WordFilter();
            var newsPreviewDivs = HtmlTools.GetTodayNews();
            foreach (var newsPreview in newsPreviewDivs)
            {
                var href = newsParser.ParseHrefOfNews(newsPreview);
                var htmlNews = HtmlTools.GetPageByUrl(href);
                var content = newsParser.ParseTextOfNews(htmlNews);
                NewsParser.CalculateStats(content, filter, frequencies);
            }
            var result = NewsParser.GetSortedDictionary(frequencies);
            foreach (var keyValuePair in result)
            {
                Console.Write(keyValuePair.Key);
                Console.Write(" ");
                Console.WriteLine(keyValuePair.Value);
            }
        }
    }
}
