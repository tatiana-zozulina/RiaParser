using System;
using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RiaParser
{
    class NewsParser
    {
        private readonly HtmlParser parser;

        public NewsParser()
        {
            parser = new HtmlParser();
        }

        public List<string> ParseNewsList(string htmlPage)
        {
            var page = parser.ParseDocument(htmlPage);
            var news = page.All
                           .Where(x => x.ClassName == "list-item")
                           .Select(x => x.OuterHtml)
                           .ToList();
            return news;
        }

        public DateTime ParseTimeOfNews(string htmlNews)
        {
            var news = parser.ParseDocument(htmlNews);
            var time = news.All
                           .Where(x => x.ClassName == "list-item__date")
                           .Select(x => x.InnerHtml)
                           .First()
                           .Split(':');
            DateTime dateTime;
            if (time[0].StartsWith("Вчера") || int.Parse(time[0]) > DateTime.Now.Hour)
            {
                dateTime = new DateTime(DateTime.Today.Year,
                                        DateTime.Today.Month,
                                        DateTime.Today.Day - 1,
                                        int.Parse(time[0].Substring(time[0].Length - 2)),
                                        int.Parse(time[1]),
                                        59);
            }
            else
            {
                dateTime = new DateTime(DateTime.Today.Year,
                                        DateTime.Today.Month,
                                        DateTime.Today.Day,
                                        int.Parse(time[0]),
                                        int.Parse(time[1]),
                                        59);
            }
            return dateTime;
        }

        public string ParseHrefOfNews(string htmlNews)
        {
            var news = parser.ParseDocument(htmlNews);
            var href = news.All
                           .Where(x => x.ClassName == "share")
                           .Select(x => x.GetAttribute("data-url"))
                           .First();
            return href;
        }

        public string ParseTextOfNews(string htmlPage)
        {
            var news = parser.ParseDocument(htmlPage);
            var text = news.All
                           .Where(x => x.ClassName == "article__text")
                           .Select(x => x.InnerHtml);
            var fullText = new StringBuilder();
            foreach (var textblock in text)
            {
                fullText.AppendLine(textblock);
            }
            return Regex.Replace(fullText.ToString(), @"<[^>]*>", " ");
        }

        public static void CalculateStats(string textOfNews, WordFilter filter, Dictionary<string, int> newsDictionary)
        {
            var wordsArray = NormalizeText(textOfNews);
            foreach (var word in wordsArray)
            {
                if (filter.IsUseless(word))
                {
                    continue;
                }
                if (newsDictionary.ContainsKey(word))
                {
                    newsDictionary[word]++;
                }
                else
                {
                    newsDictionary.Add(word, 1);
                }
            }
        }

        private static string[] NormalizeText(string text)
        {
            var noPunctuationNews = string.Concat(text.Where(c => !char.IsPunctuation(c))
                                                            .Select(c => char.ToLower(c)));
            var wordsArray = noPunctuationNews.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            return wordsArray;
        }

        public static IEnumerable<KeyValuePair<string, int>> GetSortedDictionary(Dictionary<string, int> newsDictionary)
        {
            return newsDictionary.OrderByDescending(kv => kv.Value)
                                 .Take(100);
        }

    }
}
