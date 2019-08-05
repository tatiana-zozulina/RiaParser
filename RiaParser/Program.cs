using System;
using System.Collections.Generic;
using System.Text;

namespace RiaParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var frequencies = new Dictionary<string, int>();
            var parser = new NewsParser();
            var filter = new WordFilter();
            Console.WriteLine("Getting today's news.");
            var news = HtmlTools.GetTodayNews();
            foreach (var item in news)
            {
                NewsParser.CalculateStats(item, filter, frequencies);
            }
            var result = NewsParser.GetSortedDictionary(frequencies);
            foreach (var keyValuePair in result)
            {
                Console.Write(keyValuePair.Key);
                Console.Write(" ");
                Console.WriteLine(keyValuePair.Value);
            }
            Console.ReadKey();
        }
    }
}
