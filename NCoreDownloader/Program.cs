using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCoreDownloader
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var reader = new RssReader("https://ncore.cc/rss/rssdd.xml");
			reader.ReadAsync().Wait();
        }

		
    }
}
