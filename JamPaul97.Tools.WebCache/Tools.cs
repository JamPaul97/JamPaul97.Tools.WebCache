using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JamPaul97.Tools.WebCache
{
	internal static class Tools
	{
		public static string custmomStringWebClient(string url, object trash)
		{
			string result = string.Empty;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
				result = reader.ReadToEnd();
			return result;
		}

		public static byte[] custmomByteWebClient(string url, object trash)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.AutomaticDecompression = DecompressionMethods.GZip;
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			using (var ms = new MemoryStream())
			{
				var buffer = new byte[512];
				var bytesRead = default(int);
				while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
					ms.Write(buffer, 0, bytesRead);
				return ms.ToArray();
			}
		}
	}
}
