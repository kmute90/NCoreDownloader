using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NCoreDownloader.Model
{
    public class QBitTorrentData
	{
		[Key]
		public string SessionId { get; set; }
	}
}
