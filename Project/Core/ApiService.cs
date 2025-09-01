using System;
using System.IO;
using System.Net;
using System.Text;

namespace Architexor.Core
{
	public static class ApiService
	{
		public static string PostSync(string uri, string data, string method = "POST")
		{
			/*if (uri.StartsWith("https"))
			{
				ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // SecurityProtocolType.Tls;
				ServicePointManager.Expect100Continue = true;
			}

			byte[] dataBytes = Encoding.UTF8.GetBytes(data);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.CookieContainer = new CookieContainer();
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			request.ContentLength = dataBytes.Length;
			request.Method = method;
			request.ContentType = "application/json; charset=utf-8";

			if (Constants.thisUser.Token != null && Constants.thisUser.Token != "")
			{
				request.Headers.Add("Authorization", "Bearer " + Constants.thisUser.Token);
			}
			else
			{
				//	TODO : Use constant
				request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("Architexor"));
			}

			using (Stream requestBody = request.GetRequestStream())
			{
				requestBody.Write(dataBytes, 0, dataBytes.Length);
			}

			using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using Stream stream = response.GetResponseStream();
			using StreamReader reader = new(stream);
			return reader.ReadToEnd();*/
			return "";
		}

		public static string GetResponse(string uri, string target = "")
		{
			/*HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.CookieContainer = new CookieContainer();
			request.Method = "GET";
			request.ContentType = "application/json; charset=utf-8";

			if (Constants.thisUser.Token != "")
			{
				request.Headers.Add("Authorization", "Bearer " + Constants.thisUser.Token);
			}
			else
			{
				//	TODO : Use constant
				request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("Architexor"));
			}
			//request.PreAuthenticate = true;
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			using Stream responseStream = response.GetResponseStream();
			StreamReader reader = new(responseStream, Encoding.UTF8);
			if (target == "")
				return reader.ReadToEnd();
			else
			{
				var fileStream = File.Create(target);
				reader.BaseStream.CopyTo(fileStream);
				fileStream.Close();
				return "";
			}*/
			return "";
		}
	}
}
