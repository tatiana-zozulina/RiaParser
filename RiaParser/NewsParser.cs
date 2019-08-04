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
        public static List<string> ParseNewsList(string htmlPage)
        {
            var parser = new HtmlParser();
            var page = parser.ParseDocument(htmlPage);
            var news = page.All
                           .Where(x => x.ClassName == "list-item")
                           .Select(x => x.OuterHtml)
                           .ToList();
            return news;
        }

        public static DateTime ParseTimeOfNews(string htmlNews)
        {
            var parser = new HtmlParser();
            var news = parser.ParseDocument(htmlNews);
            var time = news.All
                           .Where(x => x.ClassName == "list-item__date")
                           .Select(x => x.InnerHtml)
                           .First()
                           .Split(':');
            DateTime dateTime;
            if (time[0].StartsWith("Вчера"))
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

        public static string ParseHrefOfNews(string htmlNews)
        {
            var parser = new HtmlParser();
            var news = parser.ParseDocument(htmlNews);
            var href = news.All
                           .Where(x => x.ClassName == "share")
                           .Select(x => x.GetAttribute("data-url"))
                           .First();
            return href;
        }

        public static string ParseTextOfNews(string htmlPage)
        {
            var parser = new HtmlParser();
            var news = parser.ParseDocument(htmlPage);
            var text = news.All
                           .Where(x => x.ClassName == "article__text")
                           .Select(x => x.InnerHtml);
            var fullText = new StringBuilder();
            foreach (var textblock in text)
            {
                fullText.Append(" ");
                fullText.Append(textblock);
            }
            return Regex.Replace(fullText.ToString(), @"<[^>]*>", " ");
        }

        public static void CreateDictionary(string textOfNews, Dictionary<string, int> newsDictionary)
        {
            //var newsDictionary = new Dictionary<string, int>();
            var noPunctuationNews = string.Concat(textOfNews.Where(c => !char.IsPunctuation(c))
                                              .Select(c => char.ToLower(c)));
            var wordsArray = noPunctuationNews.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in wordsArray)
            {
                if (WordFilter.IsUseless(word))
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

            //return newsDictionary;
        }

        public static Dictionary<string, int> GetSortedDictionary(Dictionary<string, int> newsDictionary)
        {
            var tempList = newsDictionary.ToList();
            tempList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            var resultDictionary = new Dictionary<string, int>();
            for (var i = 0; i < 100; i++)
            {
                if (tempList.Count > 0)
                {
                    var last = tempList.Last();
                    resultDictionary.Add(last.Key, last.Value);
                    tempList.Remove(last);
                }
                else
                {
                    break;
                }
            }
            return resultDictionary;
        }

    }
}
