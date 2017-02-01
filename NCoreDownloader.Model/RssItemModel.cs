using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace NCoreDownloader
{
    public class RssItemModel
    {
		[Key]
		public int Id { get; set; }

		public string Title { get; set; }
		public string Link { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public string Source { get; set; }
	}
}
