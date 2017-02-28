using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq.Expressions;

namespace NCoreDownloader
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddHangfire(options => options
					.UseStorage(new PostgreSqlStorage("Host=localhost;Database=ncoredb1;Username=postgres;Password=lekvar"))
					.UseColouredConsoleLogProvider()
				)
				.AddMvc();


			var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
			services.AddSingleton(builder.Build());
			services.AddSingleton<QBitTorrentManager>();
			services.AddSingleton<NCoreDownloader.Startup>();
		}

		public void OnTimer()
		{
			//return () =>
			//{
				var reader = new RssReader("https://ncore.cc/rss/rssdd.xml");
				var items = Task.Run(reader.ReadAsync).Result;

				var downloader = new TorrentDownloader();

				DataAccess.SaveItems(items).Wait();

				var cookie = "PHPSESSID=m54jhv4al6rv082mr6vpdimr50;";
				//var qtorrent = provider.GetService<QBitTorrentManager>();
				//qtorrent.StartTorrent(new Uri(items[4].Link), cookie).Wait();
			//};
			
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider provider)
		{
			loggerFactory.AddConsole();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc(rb =>
			{
				rb.MapRoute(
					name: "default",
					template: "{controller}/{action}/{id?}",
					defaults: new { controller = "Home", action = "Index" });
			});

			app.UseHangfireServer();
			app.UseHangfireDashboard("/hangfire");

			//BackgroundJob.Schedule(() => OnTimer(), TimeSpan.FromMinutes(1));
			RecurringJob.AddOrUpdate("torrentRss", () => OnTimer(), Cron.MinuteInterval(1));

			//app.Run(async (context) =>
			//{
			//    await context.Response.WriteAsync("Hello World!");
			//});
		}
	}
}
