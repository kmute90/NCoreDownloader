using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NCoreDownloader
{
	public class RssReader
	{
		public string FeedUrl { get; private set; }

		public RssReader(string feedUrl)
		{
			FeedUrl = feedUrl;
		}

		public async Task ReadAsync()
		{
			var client = new HttpClient();
			var stream = await client.GetStreamAsync(FeedUrl);

			XmlDocument xmlFeed = new XmlDocument();
			xmlFeed.Load(stream);

			XDocument xDoc = XDocument.Parse(xmlFeed.SelectSingleNode("rss").InnerXml);
			var feeds = xDoc.Descendants("item").Select(feed => new RssItemModel()
			{
				Title = feed.Element("title").Value,
				Link = feed.Element("link").Value,
				Description = feed.Element("description").Value,
				Category = feed.Element("category").Value,
				Source = feed.Element("source").Attribute("url").Value
			});




		}



	}
}
