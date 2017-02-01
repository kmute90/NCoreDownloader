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
					context.RssItems.AddIfNotExists(item, i => i.Id == item.Id);
				}
				await context.SaveChangesAsync();
			}
		}
    }
}
