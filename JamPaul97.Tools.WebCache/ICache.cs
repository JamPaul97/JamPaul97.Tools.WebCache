using System;

namespace JamPaul97.Tools.WebCache
{
	public interface ICache
	{
		/// <summary>
		/// Checks if a cache is available for the provided URL
		/// </summary>
		/// <param name="url">URL to check</param>
		/// <returns></returns>
		bool CacheExists(string url);
		/// <summary>
		/// Try to Cache a URL as a byte array, for 'X' ammount of minutes
		/// </summary>
		/// /// <param name="url">URL to Cache</param>
		/// <param name="caller">Fucntion that will be called to get the Data from the URL</param>
		/// <param name="data">Out object to assign the result to</param>
		/// <param name="minutes">Minutes for cache to live</param>
		/// <param name="payload">Payload object to be passed to the called function</param>
		/// <param name="force">If set true and a cache is stored localy, it ignoned the cache</param>
		/// <returns>True if the request/cache read was succefull</returns>
		bool TryCacheBytes(string url, Func<string, object, byte[]> caller, out byte[] data, long minutes = 10, object payload = null, bool force = false);
		/// <summary>
		/// Try to Cache a URL as a string, for 'X' ammount of minutes
		/// </summary>
		/// <param name="url">URL to Cache</param>
		/// <param name="caller">Fucntion that will be called to get the Data from the URL</param>
		/// <param name="data">Out object to assign the result to</param>
		/// <param name="minutes">Minutes for cache to live</param>
		/// <param name="payload">Payload object to be passed to the called function</param>
		/// <param name="force">If set true and a cache is stored localy, it ignoned the cache</param>
		/// <returns>True if the request/cache read was succefull</returns>
		bool TryCacheString(string url, Func<string, object, string> caller, out string data, long minutes = 10, object payload = null, bool force = false);
		/// <summary>
		/// Try to Cache a URL as a string, for 'X' ammount of minutes, using System.Net.WebClient
		/// </summary>
		/// <param name="url">URL to Cache</param>
		/// <param name="data">Out object to assign the result to</param>
		/// <param name="minutes">Minutes for cache to live</param>
		/// <param name="force">If set true and a cache is stored localy, it ignoned the cache</param>
		/// <returns>True if the request/cache read was succefull</returns>
		bool TryCacheString(string url, out string data, long minutes = 10, bool force = false);
		/// <summary>
		/// Try to Cache a URL as a byte array, for 'X' ammount of minutes, using System.Net.WebClient
		/// </summary>
		/// <param name="url">URL to Cache</param>
		/// <param name="data">Out object to assign the result to</param>
		/// <param name="minutes">Minutes for cache to live</param>
		/// <param name="force">If set true and a cache is stored localy, it ignoned the cache</param>
		/// <returns>True if the request/cache read was succefull</returns>
		bool TryCacheBytes(string url, out byte[] data, long minutes = 10, bool force = false);
	}
}
