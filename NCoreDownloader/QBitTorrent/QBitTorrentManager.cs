using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NCoreDownloader
{
	public class QBitTorrentManager
	{
		public string SessionId { get; private set; }

		public static async Task<QBitTorrentManager> Create(string userName, string password)
		{
			using (var client = new HttpClient())
			{
				var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8080/login");
				request.Content = new StringContent($"username={userName}&password={password}", Encoding.UTF8, "application/x-www-form-urlencoded");
				var response = await client.SendAsync(request);

				var regex = new Regex(@"(?<=SID=).*(?=;)");
				var sessid = regex.Match(response.Headers.GetValues("Set-Cookie").ToArray()[0]).Value;
				return new QBitTorrentManager(sessid);
			}
		}

		QBitTorrentManager(string sessionId)
		{
			SessionId = sessionId;
		}

		/// <summary>
		/// Writes string to stream
		/// </summary>
		private void WriteToStream(Stream s, string txt)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(txt);
			s.Write(bytes, 0, bytes.Length);
		}
		

		/// <summary>
		/// Writes byte array to stream
		/// </summary>
		private void WriteToStream(Stream s, byte[] bytes)
		{
			s.Write(bytes, 0, bytes.Length);
		}


		/// <summary>
		/// Writes multi part HTTP POST request
		/// </summary>
		private void WriteMultipartForm(StringBuilder s, string boundary, Dictionary<string, string> data)
		{
			/// The first boundary
			var boundarybytes = "--" + boundary + "\r\n";
			/// the last boundary.
			var trailer = "\r\n--" + boundary + "--\r\n";
			/// the form data, properly formatted
			string formdataTemplate = "Content-Disposition: {0}\r\n\r\n{1}";

			/// Added to track if we need a CRLF or not.
			bool bNeedsCRLF = false;

			if (data != null)
			{
				foreach (string key in data.Keys)
				{
					/// if we need to drop a CRLF, do that.
					if (bNeedsCRLF)
						s.Append("\r\n");

					/// Write the boundary.
					s.Append(boundarybytes);

					/// Write the key.
					s.Append(string.Format(formdataTemplate, key, data[key]));
					bNeedsCRLF = true;
				}
			}

			/// If we don't have keys, we don't need a crlf.
			if (bNeedsCRLF)
				s.Append("\r\n");

			s.Append(boundarybytes);
			s.Append(trailer);
		}


		//public async Task StartTorrent(string path, string fileName)
		//{
		//	var cookieContainer = new CookieContainer();
		//	cookieContainer.Add(new Uri("http://localhost:8080", UriKind.Absolute), new Cookie("SID", SessionId));
		//	using (var client = new HttpClient(new HttpClientHandler() { CookieContainer = cookieContainer }))
		//	{
		//		var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8080/command/download");

		//		var boundary = $"--{DateTime.UtcNow.Ticks}--";

		//		using (Stream requestStream = new MemoryStream())
		//		{
		//			using (var fileData = File.Open(path, FileMode.Open, FileAccess.Read))
		//			{
		//				var buffer = new byte[fileData.Length];
		//				fileData.Read(buffer, 0, (int)fileData.Length);

		//				var data = new Dictionary<string, string>()
		//				{
		//					{ "form-data; name=\"urls\"", "" }
		//				};
		//				WriteMultipartForm(requestStream, boundary, data);

		//				request.Content = new StreamContent(requestStream);
		//				request.Content.Headers.Add("Content-Type", $"multipart/form-data; boundary={boundary}");

		//				var response = await client.SendAsync(request);
		//			}
		//		}

		//	}
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="torrentUrl"></param>
		/// <param name="cookie">ncore cookie</param>
		/// <returns></returns>
		public async Task StartTorrent(Uri torrentUrl, string cookie)
		{
			var cookieContainer = new CookieContainer();
			cookieContainer.Add(new Uri("http://localhost:8080", UriKind.Absolute), new Cookie("SID", SessionId));
			using (var client = new HttpClient(new HttpClientHandler() { CookieContainer = cookieContainer }))
			{
				var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8080/command/download");

				var boundary = $"--{DateTime.UtcNow.Ticks}--";

				StringBuilder builder = new StringBuilder();
				var data = new Dictionary<string, string>()
						{
							{ "form-data; name=\"urls\"", torrentUrl.AbsoluteUri },
							{ "form-data; name = \"savepath\"", "C:\\Users\\kmute\\Downloads\\" },
							{ "form-data; name = \"cookie\"", cookie}
						};
				WriteMultipartForm(builder, boundary, data);

				request.Content = new StringContent(builder.ToString(), Encoding.UTF8, $"multipart/form-data; boundary={boundary}");
				
				var response = await client.SendAsync(request);

			}
		}
	}
}
