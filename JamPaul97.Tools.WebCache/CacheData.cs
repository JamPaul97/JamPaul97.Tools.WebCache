using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JamPaul97.Tools.WebCache
{
	[Serializable]
	internal class CacheData
	{
		/// <summary>
		/// Minutes To Live
		/// </summary>
		public long MTL { get; set; }
		/// <summary>
		/// DateTime that the object was created at
		/// </summary>
		public DateTime CreatedAt { get; set; }
		/// <summary>
		/// Return trues if the object is expired
		/// </summary>
		public bool IsExpired => DateTime.Now > this.CreatedAt.AddMinutes(this.MTL);
		public CacheData(long mtl = 10)
		{
			this.MTL = mtl;
		}
	}
	[Serializable]
	internal class ByteCacheData : CacheData
	{
		/// <summary>
		/// Data of the cache object
		/// </summary>
		public byte[] data { get; set; }
		public ByteCacheData(byte[] data,long mtl=10) : base(mtl)
		{
			this.data = data;
		}
	}
	[Serializable]
	internal class StringCacheData : CacheData
	{
		/// <summary>
		/// Data of the string object
		/// </summary>
		public string data { get; set; }
		public StringCacheData(string data, long mtl = 10) : base(mtl)
		{
			this.data = data;
		}
	}
	[Serializable]
	internal class GenericCacheData<T> : CacheData
	{
		/// <summary>
		/// Data of the string object
		/// </summary>
		public T data { get; set; }
		public GenericCacheData(T data, long mtl = 10) : base(mtl)
		{
			this.data = data;
		}
	}

}
