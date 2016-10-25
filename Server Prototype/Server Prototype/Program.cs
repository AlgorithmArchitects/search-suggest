using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
			var sites = new Dictionary<string, double>();
			sites.Add("https://en.wikipedia.org/wiki/Cat", 1);
			sites.Add("https://www.cat.com/", -1);
			sites.Add("https://finance.yahoo.com/quote/cat?ltr=1", -1);
			sites.Add("http://www.vetstreet.com/cats/", .7);
			sites.Add("http://mashable.com/category/cats/", .5);
			sites.Add("https://www.google.com/finance?cid=5736", -1);
			sites.Add("http://www.catphones.com/en-us/", -1);
			sites.Add("http://www.theatlantic.com/magazine/archive/2012/03/how-your-cat-is-making-you-crazy/308873/", .5);
			sites.Add("http://www.rd.com/advice/pets/how-to-decode-your-cats-behavior/", .3);
			sites.Add("http://www.nybooks.com/articles/2016/09/29/killer-cats-are-winning/", .5);

			var res = KeywordExtractor.ExtractKeywords(sites);

			var myList = res.ToList();

			myList.Sort(
				delegate (KeyValuePair<string, double> pair1,
				KeyValuePair<string, double> pair2)
				{
					return pair1.Value.CompareTo(pair2.Value);
				}
			);
			for(int i = 0; i < 5; i++)
			{
				Console.WriteLine($"-{myList[i]}");
			}
			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine($"+{myList[myList.Count-1-i]}");
			}
			while (true) ;
		}
    }
}
