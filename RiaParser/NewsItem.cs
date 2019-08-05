using System;

namespace RiaParser
{
    class NewsItem
    {
        public readonly string link;
        public readonly DateTime dateTime;

        public string Text { get; private set; }

        public NewsItem(string link, DateTime dateTime)
        {
            this.link = link;
            this.dateTime = dateTime;
        }

        public void DownloadContent(NewsParser parser, string htmlPage)
        {
            Text = parser.ParseTextOfNews(htmlPage);
        }

    }
}
