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
            var dict = new Dictionary<string, int>();
            var temp = HtmlTools.GetTodayNews();
            foreach (var t in temp)
            {
                var href = NewsParser.ParseHrefOfNews(t);
                var time = NewsParser.ParseTimeOfNews(t);
                var htmlNews = HtmlTools.GetPageByUrl(href);
                var innerNews = NewsParser.ParseTextOfNews(htmlNews);
                NewsParser.CreateDictionary(innerNews, dict);
            }
            var result = NewsParser.GetSortedDictionary(dict);
            foreach (var keyValuePair in result)
            {
                Console.Write(keyValuePair.Key);
                Console.Write(":");
                Console.WriteLine(keyValuePair.Value);
            }
            Console.Write("END");
            Console.ReadKey();
        }
    }
}
