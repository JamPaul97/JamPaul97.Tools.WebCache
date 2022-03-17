using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JamPaul97.Tools.WebCache
{
	public class MemoryCache : ICache
	{
		private Dictionary<string, StringCacheData> _stringCache;
		private Dictionary<string, ByteCacheData> _byteCache;
		/// <summary>
		/// Create a new MemoryCache
		/// </summary>
		public MemoryCache()
		{
			this._byteCache = new Dictionary<string, ByteCacheData>();
			this._stringCache = new Dictionary<string, StringCacheData>();
		}
		
		/// <summary>
		/// Create a new MemoryCache, with a previus backup
		/// </summary>
		/// <param name="filename">File from previus backup</param>
		public MemoryCache(string filename)
		{
			try
			{
				using(var fs = new FileStream(filename, FileMode.Open))
				{
					var obj = (backupClass)new BinaryFormatter().Deserialize(fs);
					this._byteCache = obj.Bytes ?? new Dictionary<string, ByteCacheData>();
					this._stringCache = obj.Strings ?? new Dictionary<string, StringCacheData>();
				}
			}catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <inheritdoc />
		public bool CacheExists(string url)
		{
			if (this._stringCache.ContainsKey(url))
				return true;
			else if (this._byteCache.ContainsKey(url))
				return true;
			return false;
		}

        #region Sync

        /// <inheritdoc />
        public bool TryCacheBytes(string url, Func<string, object, byte[]> caller, out byte[] data, long minutes = 10, object payload = null, bool force = false)
		{
			if (this._byteCache.ContainsKey(url) && !force)
			{
				var cache = this._byteCache[url];
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
				var cache = new ByteCacheData(response, minutes);
				if (this._byteCache.ContainsKey(url))
					this._byteCache.Remove(url);
				this._byteCache.Add(url, cache);
				data = response;
				return true;
			}
		}

		/// <inheritdoc />
		public bool TryCacheString(string url, Func<string, object, string> caller, out string data, long minutes = 10, object payload = null, bool force = false)
		{
			if (this._stringCache.ContainsKey(url) && !force)
			{
				var cache = this._stringCache[url];
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
				if (this._stringCache.ContainsKey(url))
					this._stringCache.Remove(url);
				this._stringCache.Add(url, cache);
				data = response;
				return true;
			}
		}

		/// <inheritdoc />
		public bool TryCacheString(string url, out string data, long minutes = 10, bool force = false)=>
			TryCacheString(url, Tools.custmomStringWebClient, out data, minutes,null, force);
			
		/// <inheritdoc />
		public bool TryCacheBytes(string url, out byte[] data, long minutes = 10, bool force = false) =>
			TryCacheBytes(url, Tools.custmomByteWebClient, out data, minutes,null, force);

		
		#endregion

		/// <summary>
		/// Saves the current Memory caches to a file, you can load the backup file later to a new MemoryCache
		/// </summary>
		/// <param name="filename">Filename that the cache entries will be savesdd</param>
		/// <returns></returns>
		public long Backup(string filename)
		{
			var obj = new backupClass
			{
				Strings = this._stringCache,
				Bytes = this._byteCache
			};
			using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
			{
				new BinaryFormatter().Serialize(fs, obj);
				return fs.Length;
			}
		}

		#region Async

		public delegate void DownloadProgressEvent(string uuid, DownloadProgressChangedEventArgs args);
		public delegate void DownloadStringCompleted(string uuid, DownloadStringCompletedEventArgs e);
		public delegate void DownloadBytesCompleted(string uuid, DownloadDataCompletedEventArgs e);
		public event DownloadProgressEvent OnDownloadStringProgress;
		public event DownloadStringCompleted OnDownloadStringCompleted;
		public event DownloadBytesCompleted OnDownloadBytesCompleted;

		private List<AsyncModel> asyncWBs = new List<AsyncModel>();

		public string TryCacheStringAsync(string url, out string data, long minutes=10, bool force=false)
		{
			if (this._stringCache.ContainsKey(url) && !force)
			{
				var cache = this._stringCache[url];
				if (!cache.IsExpired)
				{
					data = cache.data;
					return string.Empty;
				}
			}
			var wb = new WebClient();
			var uuid = Guid.NewGuid().ToString();
            wb.DownloadStringCompleted += Wb_DownloadStringCompleted;
			wb.DownloadProgressChanged += Wb_DownloadProgressChanged;
			var _d = new AsyncModel()
			{
				wb = wb,
				minutes = minutes,
				force = force,
				url = url,
				uuid = uuid
			};
			asyncWBs.Add(_d);
			wb.DownloadStringAsync(new Uri(url));
			data = string.Empty;
			return uuid;
		}

        private void Wb_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
			if (this.OnDownloadStringProgress == null)
				return;
			var wb = sender as WebClient;
			if (asyncWBs.Any(x => x.wb == wb))
			{
				var uuid = asyncWBs.First(x => x.wb == wb).uuid;
				this.OnDownloadStringProgress(uuid, e);
			}
		}

        private void Wb_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
			var wb = sender as WebClient;
			if (asyncWBs.Any(x => x.wb == wb))
			{
				var asyncObj = asyncWBs.First(x => x.wb == wb);
				//Cache file
				if (!e.Cancelled)
				{
					var cache = new StringCacheData(e.Result, asyncObj.minutes);
					if (this._stringCache.ContainsKey(asyncObj.url))
						this._stringCache.Remove(asyncObj.url);
					this._stringCache.Add(asyncObj.url, cache);
				}
				if (this.OnDownloadStringCompleted != null)
					this.OnDownloadStringCompleted(asyncObj.uuid, e);
				asyncWBs.RemoveAll(x => x.wb == wb);
			}
		}

        public string TryCacheBytesAsync(string url, out byte[] data, long minutes = 10, bool force = false)
		{
			if (this._stringCache.ContainsKey(url) && !force)
			{
				var cache = this._byteCache[url];
				if (!cache.IsExpired)
				{
					data = cache.data;
					return string.Empty;
				}
			}
			var wb = new WebClient();
			var uuid = Guid.NewGuid().ToString();
            wb.DownloadDataCompleted += Wb_DownloadDataCompleted;
			wb.DownloadProgressChanged += Wb_DownloadProgressChanged;
			var _d = new AsyncModel()
			{
				wb = wb,
				minutes = minutes,
				force = force,
				url = url,
				uuid = uuid
			};
			asyncWBs.Add(_d);
			wb.DownloadStringAsync(new Uri(url));
			data = null;
			return uuid;
		}

        private void Wb_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
			var wb = sender as WebClient;
			if (asyncWBs.Any(x => x.wb == wb))
			{
				var asyncObj = asyncWBs.First(x => x.wb == wb);
				//Cache file
				if (!e.Cancelled)
				{
					var cache = new ByteCacheData(e.Result, asyncObj.minutes);
					if (this._byteCache.ContainsKey(asyncObj.url))
						this._byteCache.Remove(asyncObj.url);
					this._byteCache.Add(asyncObj.url, cache);
				}
				if (this.OnDownloadBytesCompleted != null)
					this.OnDownloadBytesCompleted(asyncObj.uuid, e);
				asyncWBs.RemoveAll(x => x.wb == wb);
			}
		}


        #endregion




        [Serializable]
		private class backupClass
		{
			public Dictionary<string, StringCacheData> Strings;
			public Dictionary<string, ByteCacheData> Bytes;
		}

	}
}
