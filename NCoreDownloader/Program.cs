using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreDownloader
{
    public class Program
    {
        public static void Main(string[] args)
        {
			IServiceCollection serviceCollection = new ServiceCollection();
			var task = ConfigureServices(serviceCollection);

			task.Wait();
			var provider = task.Result;
			//Application application = new Application(serviceCollection);


			// Create a IPC wait handle with a unique identifier.
			bool createdNew;
			var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, Guid.NewGuid().ToString().ToUpper(), out createdNew);
			var signaled = false;

			// If the handle was already there, inform the other process to exit itself.
			// Afterwards we'll also die.
			if (!createdNew)
			{
				waitHandle.Set();
				return;
			}

			// Start a another thread that does something every 10 seconds.
			var timer = new Timer(OnTimerElapsed, provider, TimeSpan.Zero, TimeSpan.FromSeconds(60));

			// Wait if someone tells us to die or do every five seconds something else.
			do
			{
				signaled = waitHandle.WaitOne(TimeSpan.FromSeconds(5));
				// ToDo: Something else if desired.
			} while (!signaled);

			// The above loop with an interceptor could also be replaced by an endless waiter
			//waitHandle.WaitOne();
			

		}

		static private async Task<IServiceProvider> ConfigureServices(IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton(await QBitTorrentManager.Create("admin", "Lekvar$13"));
			serviceCollection.BuildServiceProvider();
			return serviceCollection.BuildServiceProvider();
			//serviceCollection.AddInstance<ILoggerFactory>(loggerFactory);
		}

		private static void OnTimerElapsed(object state)
		{
			var provider = (IServiceProvider)state;
			var reader = new RssReader("https://ncore.cc/rss/rssdd.xml");
			var items = reader.ReadAsync().Result;
				
			
			var downloader = new TorrentDownloader();
			//var download = downloader.DownloadTorrent(items[0].Link);
			//download.Wait();
			//var filePath = download.Result;

			DataAccess.SaveItems(items).Wait();

			var cookie = "PHPSESSID=horlprh44n1kqo31oiqu5r4t91; adblock_stat=1; __utmt=1; nick=kmute90; pass=f4f9b55ada2b007133499906788d96ec; stilus=mousy; nyelv=hu; __utma=82829833.572263546.1486233890.1486233890.1486240565.2; __utmb=82829833.2.10.1486240565; __utmc=82829833; __utmz=82829833.1486233890.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); adblock_tested=false";
			var qtorrent = provider.GetService<QBitTorrentManager>();
			qtorrent.StartTorrent(new Uri(items[0].Link), cookie).Wait();
		}
	}
}
