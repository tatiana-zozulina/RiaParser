using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;

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

        public static IEnumerable<string> GetTodayNews()
        {
            var lastTime = DateTime.Now;
            var today = DateTime.Today.Day;
            var newsParser = new NewsParser();
            var newsHtml = new HashSet<string>();
            do
            {
                var link = GenerateLink(DateTime.Now);
                var data = GetPageByUrl(link);
                var page = newsParser.ParseNewsList(data);
                foreach(var x in page)
                {
                    if (!newsHtml.Contains(x))
                    {
                        newsHtml.Add(x);
                    }
                }
                lastTime = newsParser.ParseTimeOfNews(page.Last());
            }
            while (lastTime.Day == today);

            return newsHtml.Where(x => newsParser.ParseTimeOfNews(x).Day == today);
        }
    }
}
