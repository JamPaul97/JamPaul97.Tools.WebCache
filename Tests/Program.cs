using JamPaul97.Tools.WebCache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var cacher = new MemoryCache();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var uuid = cacher.TryCacheString("https://github.com/json-iterator/test-data/blob/master/large-file.json?raw=true", get, out string data,10);
            sw.Stop();
            Console.WriteLine($"Custom Caller took  : {(float)(sw.ElapsedMilliseconds/1000.0)}s");
            sw.Restart();
            uuid = cacher.TryCacheString("https://github.com/json-iterator/test-data/blob/master/large-file.json?raw=true",get, out string data2);
            sw.Stop();
            Console.WriteLine($"Custom Caller cache took : {(float)(sw.ElapsedMilliseconds / 1000.0)}s");

            sw.Restart();
            uuid = cacher.TryCacheString("https://github.com/json-iterator/test-data/blob/master/large-file.json?raw=true",out string data3,10,true);
            sw.Stop();
            Console.WriteLine($"Default Called took : {(float)(sw.ElapsedMilliseconds / 1000.0)}s");
            sw.Restart();
            uuid = cacher.TryCacheString("https://github.com/json-iterator/test-data/blob/master/large-file.json?raw=true", out string data4, 10);
            sw.Stop();
            Console.WriteLine($"Default Called cache took : {(float)(sw.ElapsedMilliseconds / 1000.0)}s");

            var a = Console.ReadLine();
        }
        public static string get(string url,object payload = null)
		{
            string html = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.None;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }
    }
}
