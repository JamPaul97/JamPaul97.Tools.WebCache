using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JamPaul97.Tools.WebCache
{
	public class FileCache : ICache
	{
	    private string haskKey(string key)
		{
			using (SHA1 hasher = SHA1.Create())
			{
				byte[] input = Encoding.UTF8.GetBytes(key);
				byte[] hashBytes = hasher.ComputeHash(input);
				return BitConverter.ToString(hashBytes).Replace("-", "");
			}
		}

		protected string _cacheFolder;

		/// <summary>
		/// Create a new FileCache
		/// </summary>
		/// <param name="Folder">Directory that the cache will be saved</param>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public FileCache(string Folder)
		{
			if (!Directory.Exists(Folder))
			{
				Directory.CreateDirectory(Folder);
				if (!Directory.Exists(Folder))
					throw new DirectoryNotFoundException();
			}
			_cacheFolder = Folder;
			if (!_cacheFolder.EndsWith("\\"))
				_cacheFolder += "\\";
		}

		/// <summary>
		/// Get the filename of the Cache
		/// </summary>
		/// <param name="url">The URL of the online resource</param>
		/// <returns>The filename under witch the cache data is saved</returns>
		public string Path(string url) => this._cacheFolder + haskKey(url) + ".bin";

		/// <inheritdoc />
		public bool CacheExists(string url) => File.Exists(this.Path(url));

		/// <summary>
		/// Try to Cache a URL as a byte array, until this function is called again with the 'force' parameter as true
		/// </summary>
		/// <param name="url">URL to Cache</param>
		/// <param name="caller">Fucntion that will be called to get the Data from the URL</param>
		/// <param name="data">Out object to assign the result to</param>
		/// <param name="payload">Payload object to be passed to the called function</param>
		/// <param name="force">If set true and a cache is stored localy, it ignoned the cache</param>
		/// <returns>True if the request/cache read was succefull</returns>
		public bool TryDeepCacheBytes(string url,Func<string,object,byte[]> caller, out byte[] data, object payload = null,bool force = false)
		{
			var filename = this.Path(url);
			if (File.Exists(filename) && !force)
			{
				data = File.ReadAllBytes(filename);
				return true;
			}
			byte[] response = caller(url, payload);			
			if (response == null)
			{
				data = null;
				return false;
			}
			//Cache
			File.WriteAllBytes(filename, response);
			data = response;
			return true;
		}

		/// <summary>
		/// Try to Cache a URL as a string, until this function is called again with the 'force' parameter as true
		/// </summary>
		/// <param name="url">URL to Cache</param>
		/// <param name="caller">Fucntion that will be called to get the Data from the URL</param>
		/// <param name="data">Out object to assign the result to</param>
		/// <param name="payload">Payload object to be passed to the called function</param>
		/// <param name="force">If set true and a cache is stored localy, it ignoned the cache</param>
		/// <returns>True if the request/cache read was succefull</returns>
		public bool TryDeepCacheString(string url, Func<string, object, string> caller, out string data, object payload = null, bool force = false)
		{
			var filename = this.Path(url);
			if (File.Exists(filename) && !force)
			{
				data = File.ReadAllText(filename);
				return true;
			}
			var response = caller(url, payload);
			if (response == null)
			{
				data = null;
				return false;
			}
			//Cache
			File.WriteAllText(filename, response);
			data = response;
			return true;
		}

		/// <inheritdoc />
		public bool TryCacheBytes(string url, Func<string, object, byte[]> caller, out byte[] data,long minutes = 10, object payload = null, bool force = false)
		{
			var filename = this.Path(url);
			if (File.Exists(filename) && !force)
			{
				var cache = this.getCache(filename) as ByteCacheData;
				if(!cache.IsExpired)
				{
					data = cache.data;
					return true;
				}
			}
			var response = caller(url, payload);
			if (response == null)
			{
				data = null;
				return false;
			}
			//Cache
			{
				var cache = new ByteCacheData(response, minutes);
				this.saveCache(cache,filename);
				data = response;
				return true;
			}
		}

		/// <inheritdoc />
		public bool TryCacheString(string url, Func<string, object, string> caller, out string data, long minutes = 10, object payload = null, bool force = false)
		{
			var filename = this.Path(url);
			if (File.Exists(filename) && !force)
			{
				var cache = this.getCache(filename) as StringCacheData;
				if (!cache.IsExpired)
				{
					data = cache.data;
					return true;
				}
			}
			var response = caller(url, payload);
			if (response == null)
			{
				data = null;
				return false;
			}
			//Cache
			{
				var cache = new StringCacheData(response, minutes);
				this.saveCache(cache, filename);
				data = response;
				return true;
			}
		}

		/// <inheritdoc />
		public bool TryCacheString(string url, out string data, long minutes = 10, bool force = false)
		{
			var filename = this.Path(url);
			if (File.Exists(filename) && !force)
			{
				var cache = this.getCache(filename) as StringCacheData;
				if (!cache.IsExpired)
				{
					data = cache.data;
					return true;
				}
			}
			var response = new WebClient().DownloadString(url);
			if (response == null)
			{
				data = null;
				return false;
			}
			//Cache
			{
				var cache = new StringCacheData(response, minutes);
				this.saveCache(cache, filename);
				data = response;
				return true;
			}
		}

		/// <inheritdoc />
		public bool TryCacheBytes(string url, out byte[] data, long minutes = 10, bool force = false) =>
		TryCacheBytes(url, this.custmomByteWebClient, out data, minutes, force);

		/// <summary>
		/// Try to Cache a URL as a byte array, until this function is called again with the 'force' parameter as true
		/// </summary>
		/// <param name="url">URL to Cache</param>
		/// <param name="data">Out object to assign the result to</param>
		/// <param name="force">If set true and a cache is stored localy, it ignoned the cache</param>
		/// <returns>True if the request/cache read was succefull</returns>
		public bool TryDeepCacheByte(string url, out byte[] data, long minutes = 10, bool force = false)=>
			TryDeepCacheBytes(url,this.custmomByteWebClient,out data,minutes, force);
		
		/// <summary>
		/// Try to Cache a URL as a string, until this function is called again with the 'force' parameter as true
		/// </summary>
		/// <param name="url">URL to Cache</param>
		/// <param name="data">Out object to assign the result to</param>
		/// <param name="force">If set true and a cache is stored localy, it ignoned the cache</param>
		/// <returns>True if the request/cache read was succefull</returns>
		public bool TryDeepCacheString(string url, out string data, long minutes = 10, bool force = false)=>
			TryDeepCacheString(url,this.custmomStringWebClient,out data,minutes,force);

		private string custmomStringWebClient(string url,object trash)
		{
			var wb = new WebClient();
			return wb.DownloadString(url);
		}
		private byte[] custmomByteWebClient(string url, object trash)
		{
			var wb = new WebClient();
			return wb.DownloadData(url);
		}


		private CacheData getCache(string filename)
		{
			if (!File.Exists(filename)) throw new FileNotFoundException();
			using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
				return (CacheData)new BinaryFormatter().Deserialize(fs);
		}
		
		private void saveCache(CacheData data,string filename)
		{
			using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
				new BinaryFormatter().Serialize(fs, data);
		}

	}
}
