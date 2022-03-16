using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JamPaul97.Tools.WebCache
{
    public static class Class1
    {
        public static void Test(string url,System.Action f)
		{
            f();		
        }
        public static void a()
		{
            string re = string.Empty;
            Test("", ()=>b(2,1,out re));
            Console.WriteLine(re);
            
		}
        public static void b(int a, int b,out string re)
		{
            re = "asd";
            Console.WriteLine(re);
            Console.WriteLine(a.ToString());
            Console.WriteLine(b.ToString());
		}
    }
}
