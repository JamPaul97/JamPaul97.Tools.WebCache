using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JamPaul97.Tools.WebCache
{
    internal class AsyncModel
	{
		public WebClient wb;
		public long minutes;
		public bool force;
		public string url;
		public string uuid;
		public string filename;
	}
}
