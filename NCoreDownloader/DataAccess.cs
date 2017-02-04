using Microsoft.EntityFrameworkCore;
using NCoreDownloader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCoreDownloader
{
    public class DataAccess
    {
		public static async Task SaveItems(List<RssItemModel> items)
		{
			using (var context = new NCoreDownloaderContext())
			{
				foreach (var item in items)
				{
					await context.RssItems.AddIfNotExists(item, i => i.Id == item.Id);
				}
				await context.SaveChangesAsync();
			}
		}

		public static async Task SaveSessionId(string sessionId)
		{
			using (var context = new NCoreDownloaderContext())
			{
				//korábbi cookie törlése
				var data = await context.QBitTorrentData.ToListAsync();
				context.QBitTorrentData.RemoveRange(data);

				var entity = new QBitTorrentData()
				{
					SessionId = sessionId
				};
				context.QBitTorrentData.Add(entity);
				await context.SaveChangesAsync();
			}
		}
    }
}
