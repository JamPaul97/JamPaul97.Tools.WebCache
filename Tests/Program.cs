using JamPaul97.Tools.WebCache;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cacher = new MemoryCache("directory_to_Save_files");
            if (cacher.TryCacheBytes("https://ip-fast.com/api/ip/", out byte[] data))
            {
                Image img = Image.FromStream(new MemoryStream(data));
            }



        }
        class PayloadObject
        {
            public string username { get; set; }
            public string password { get; set; }
        }
        public string MakeHTTPRequest(string url, object payload)
        {
            var _payload = payload as PayloadObject;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(_payload);
                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
