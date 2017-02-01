using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCoreDownloader
{
    public class RssItemModel
    {
		public string Title { get; set; }
		public string Link { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public string Source { get; set; }
	}
}
