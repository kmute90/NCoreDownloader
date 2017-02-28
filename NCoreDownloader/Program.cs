using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace NCoreDownloader
{
    public class Program
    {
        public static void Main(string[] args)
        {
			IServiceCollection serviceCollection = new ServiceCollection();

			var provider = ConfigureServices(serviceCollection);
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

		static private IServiceProvider ConfigureServices(IServiceCollection serviceCollection)
		{
			var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
			serviceCollection = serviceCollection.AddSingleton(builder.Build());
			serviceCollection = serviceCollection.AddSingleton<QBitTorrentManager>();
			
			return serviceCollection.BuildServiceProvider();
		}

		private static void OnTimerElapsed(object state)
		{
			var provider = (IServiceProvider)state;
			var reader = new RssReader("https://ncore.cc/rss/rssdd.xml");
			var items = reader.ReadAsync().Result;				
			
			var downloader = new TorrentDownloader();

			DataAccess.SaveItems(items).Wait();

			var cookie = "PHPSESSID=m54jhv4al6rv082mr6vpdimr50;";
			var qtorrent = provider.GetService<QBitTorrentManager>();
			qtorrent.StartTorrent(new Uri(items[4].Link), cookie).Wait();
		}
	}
}
