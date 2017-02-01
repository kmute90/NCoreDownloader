using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NCoreDownloader
{
    public class TorrentDownloader
    {
		public TorrentDownloader()
		{
			
		}

		public async Task DownloadTorrent(string url)
		{
			using (var client = new HttpClient())
			{
				var request = new HttpRequestMessage()
				{
					RequestUri = new Uri(url),
					Method = HttpMethod.Get,
				};
				request.Headers.Add("Cookie", "");


				var response = await client.SendAsync(request);
				var stream = await response.Content.ReadAsStreamAsync();
				var contentDisposition = response.Content.Headers.Where(h => h.Key == "Content-Disposition").Select(h => h.Value).First().First();
				var regex = new Regex("(?<=filename=\").*(?=\")");
				var fileName = "C:\\torrents\\" + regex.Match(contentDisposition).Value;
				try
				{
					if(!File.Exists(fileName))
					using (var fileStream = File.Create(fileName))
					{
						stream.Seek(0, SeekOrigin.Begin);
						stream.CopyTo(fileStream);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}
    }
}
