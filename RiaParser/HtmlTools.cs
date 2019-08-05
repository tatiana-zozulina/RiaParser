using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiaParser
{
    class HtmlTools
    {
        public static string GetPageByUrl(string urlAddress)
        {
            var request = (HttpWebRequest)WebRequest.Create(urlAddress);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    var encoding = Encoding.GetEncoding(response.CharacterSet);

                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(responseStream, encoding))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        public static string GenerateLink(DateTime dateTime)
        {
            var url = @"https://ria.ru/services/lenta/more.html?date=";
            return $"{url}{dateTime:yyyyMMddTHHmmss}";
        }

        public static IEnumerable<NewsItem> GetTodayNews()
        {
            var lastTime = DateTime.Now;
            var today = DateTime.Today.Day;
            var parser = new NewsParser();
            var newsHtml = new HashSet<NewsItem>();
            do
            {
                var href = GenerateLink(lastTime);
                var data = GetPageByUrl(href);
                var page = parser.ParseNewsList(data);
                foreach(var singlePreview in page)
                {
                    var link = parser.ParseHrefOfNews(singlePreview);
                    lastTime = parser.ParseTimeOfNews(singlePreview);
                    var htmlPage = GetPageByUrl(link);
                    var item = new NewsItem(link, lastTime);
                    item.DownloadContent(parser, htmlPage);
                    if (!newsHtml.Contains(item))
                    {
                        newsHtml.Add(item);
                    }
                }
            }
            while (lastTime.Day == today);

            return newsHtml.Where(x => x.dateTime.Day == today);
        }
    }
}
