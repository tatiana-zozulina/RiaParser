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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream recieveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(recieveStream);
                }
                else
                {
                    readStream = new StreamReader(recieveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                //using (StreamWriter fileStream = File.AppendText(filePath))
                //{
                //    fileStream.Write(data);
                //    fileStream.Close();
                //}
                return data;
            }
            return null;
        }

        public static string GenerateLink(DateTime dateTime)
        {
            var url = @"https://ria.ru/services/lenta/more.html?date=";
            var date = dateTime.ToString("yyyyMMdd");
            var time = dateTime.ToString("HHmmss");
            return url + date + "T" + time;
        }

        public static IEnumerable<string> GetTodayNews()
        {
            var link = GenerateLink(DateTime.Now);
            var data = GetPageByUrl(link);
            var news = NewsParser.ParseNewsList(data);
            var time = NewsParser.ParseTimeOfNews(news.Last());

            while (time.Day == DateTime.Today.Day)
            {
                //System.Threading.Thread.Sleep(1000);
                link = GenerateLink(time);
                data = GetPageByUrl(link);
                var temp = NewsParser.ParseNewsList(data);
                foreach (var n in temp)
                {
                    if (!news.Contains(n))
                    {
                        news.Add(n);
                    }
                }
                time = NewsParser.ParseTimeOfNews(news.Last());
            }

            while (NewsParser.ParseTimeOfNews(news.First()).Day == DateTime.Today.Day)
            {
                yield return news.First();
                news.Remove(news.First());
            }
            yield break;
            //return news;
        }
    }
}
