
# JamPaul97.Tools.WebCache
WebCache is a C# library that helps you manage your HTTP requests response's cache

# Features
  File Cache
  Memory Cache
  Synchronous API 
  Asynchronous API
  Custom HTTP Caller

# How to install
  Install RiotSharp through NuGet:
  ```
  PM > Install JamPaul97.Tools.WebCache
  ```
  OR
  Download the latest .dll file from the Releases pages and add it to your project as a reference.
  OR
  Clonse and build this repo using Visual Studio >19

# Usage
### Main API
There are 2 main classes you can use for your cache. Firstly is the **FileCache**, that is caching the data to the disk, on a selected directory. And secondly there is the MemoryCache, that is caching the data on RAM. Please note that if you use MemoryCache the cache will be alive until you exit your app.

#### File Cacher Example
```c#
using JamPaul97.Tools.WebCache;

var cacher = new FileCache("directory_to_Save_files");
if (cacher.TryCacheString("https://ip-fast.com/api/ip/", out string ip))
	Console.WriteLine($"My IP is : {ip}");
else Console.WriteLine("Error getting my IP");
```
#### Memory Cacher Exampe
```c#
using JamPaul97.Tools.WebCache;

var cacher = new MemoryCache("directory_to_Save_files");
if (cacher.TryCacheString("https://ip-fast.com/api/ip/", out string ip))
	Console.WriteLine($"My IP is : {ip}");
else Console.WriteLine("Error getting my IP");
```

As you can see both caches are similar in use. This library can be used the same way for both methods.

#### Cache Bytes
Using the method TryCacheBytes you can download the resource as bytes array, and use it as you desire. The example bellow shows you how to treat the HTTP request as an image.
```c#
var cacher = new MemoryCache("directory_to_Save_files");
if (cacher.TryCacheBytes("https://http.cat/401", out byte[] data))
	Image img = Image.FromStream(new MemoryStream(data));
```
# Custom HTTP Caller
Let's say you have your own custom function that make a POST request to url with some JSON payload like : 
```c#
public static string MakeHTTPRequest(string url, object payload)
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
```
The JsonObject class is : 
```c#
class PayloadObject
{
  public string username { get; set; }
  public string password { get; set; }
}
 ```
 
 So we can make the Cache as : 
 ```c#
 
 var cacher = new MemoryCache("directory_to_Save_files");
var Payload = new PayloadObject()
{
	username = "MyUsername",
	password = "MyPassword"
};
if (cacher.TryCacheString("https://ip-fast.com/api/ip/", MakeHTTPRequest, out string data, 10, Payload))
{
	//data contains the result
}
```

# Async String

```c#
var cacher = new MemoryCache("directory_to_Save_files");
string uuid = cacher.TryCacheStringAsync("YOUR_URL", out string data);
if (data != string.Empty) {
  //data contains the cached result
  //uuid is string.Empty
  handleData(data);
} else {
  cacher.OnDownloadStringCompleted += Cacher_OnDownloadStringCompleted;
  cacher.OnDownloadStringProgress += Cacher_OnDownloadStringProgress;
}

private static void Cacher_OnDownloadStringProgress(string uuid, DownloadProgressChangedEventArgs args) {
  //handle download progress
}

private static void Cacher_OnDownloadStringCompleted(string uuid, DownloadStringCompletedEventArgs e) {
  //handle download clompleted
  handleData(e.Data);
}

private void handleData(string data){
  //handle your data
}
```

As you can see when trying to async cache, the function is returning a string UUID. You have to store this UUID because when the ```OnDownloadStringProgress``` event fires it will contain the UUID of the completed request, this way you can have multiple caches running at the same time, and distinguish between them using that UUID.
 
 
