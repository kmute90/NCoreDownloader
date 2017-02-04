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

		public async Task<Tuple<string, string>> DownloadTorrent(string url)
		{
			using (var client = new HttpClient())
			{
				var request = new HttpRequestMessage()
				{
					RequestUri = new Uri(url),
					Method = HttpMethod.Get,
				};
				request.Headers.Add("Cookie", "PHPSESSID=horlprh44n1kqo31oiqu5r4t91; adblock_stat=1; __utmt=1; nick=kmute90; pass=f4f9b55ada2b007133499906788d96ec; stilus=mousy; nyelv=hu; __utma=82829833.572263546.1486233890.1486233890.1486240565.2; __utmb=82829833.2.10.1486240565; __utmc=82829833; __utmz=82829833.1486233890.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); adblock_tested=false");


				var response = await client.SendAsync(request);
				var stream = await response.Content.ReadAsStreamAsync();
				var contentDisposition = response.Content.Headers.Where(h => h.Key == "Content-Disposition").Select(h => h.Value).First().First();
				var regex = new Regex("(?<=filename=\").*(?=\")");
				var fileName = regex.Match(contentDisposition).Value;
				var path = "C:\\torrents\\" + fileName;
				try
				{
					if(!File.Exists(fileName))
					using (var fileStream = File.Create(path))
					{
						stream.Seek(0, SeekOrigin.Begin);
						stream.CopyTo(fileStream);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

				return new Tuple<string, string>(fileName, path);
			}
		}
    }
}
