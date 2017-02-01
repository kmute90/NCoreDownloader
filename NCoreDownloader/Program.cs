using System;
using System.Threading;

namespace NCoreDownloader
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
			var timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

			// Wait if someone tells us to die or do every five seconds something else.
			do
			{
				signaled = waitHandle.WaitOne(TimeSpan.FromSeconds(5));
				// ToDo: Something else if desired.
			} while (!signaled);

			// The above loop with an interceptor could also be replaced by an endless waiter
			//waitHandle.WaitOne();
			

		}

		private static void OnTimerElapsed(object state)
		{
			var reader = new RssReader("https://ncore.cc/rss/rssdd.xml");
			var items = reader.ReadAsync().Result;

			var downloader = new TorrentDownloader();
			downloader.DownloadTorrent(items[0].Link).Wait();

			DataAccess.SaveItems(items).Wait();

		}
	}
}
