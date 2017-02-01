using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreDownloader.Web
{
    public class WebSocketListener
    {
		public WebSocketListener()
		{			
		}

		public async Task Listen()
		{
			using (var socket = new ClientWebSocket())
			{
				await socket.ConnectAsync(new Uri("push.ncore.cc"), CancellationToken.None);
			}
		}
    }
}
