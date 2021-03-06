﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NCoreDownloader.Model;

namespace NCoreDownloader
{
    public class NCoreDownloaderContext : DbContext
    {
		public DbSet<RssItemModel> RssItems { get; set; }
		public DbSet<QBitTorrentData> QBitTorrentData { get; set; }

		public NCoreDownloaderContext()
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(@"Host=localhost;Database=ncoredb;Username=ncoredb;Password=12ncoredb21");
		}
	}
}
