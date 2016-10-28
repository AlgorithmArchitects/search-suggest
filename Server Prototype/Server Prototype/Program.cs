using System;
using System.Collections.Generic;
using System.Linq;

namespace KeywordExtractorServer
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

			DisplayResults(res);
			Console.WriteLine("\n");

			sites = new Dictionary<string, double>();

			sites.Add("https://www.marshalltown.k12.ia.us/", 0);
			sites.Add("https://twitter.com/mcsdbobcats", 0);
			sites.Add("https://www.marshalltown.k12.ia.us/schools/miller-middle-school/", 0);
			sites.Add("http://ci.marshalltown.ia.us/", 0);
			sites.Add("https://marshalltown.com/", 0);
			sites.Add("https://en.wikipedia.org/wiki/Marshalltown,_Iowa", 0);
			sites.Add("http://www.visitmarshalltown.com/", 0);
			sites.Add("http://www.timesrepublican.com/", 0);
			sites.Add("https://www.marshalltown.org/", 0);
			sites.Add("https://mcc.iavalley.edu/", 0);

			res = KeywordExtractor.ExtractKeywords(sites);

			DisplayResults(res);

			while (true) ;
		}

		private static void DisplayResults(Dictionary<string, double> results)
		{
			var myList = results.ToList();

			myList.Sort(
				delegate (KeyValuePair<string, double> pair1,
				KeyValuePair<string, double> pair2)
				{
					return pair1.Value.CompareTo(pair2.Value);
				}
			);
			for (int i = 0; i < 5 && i < myList.Count; i++)
			{
				Console.WriteLine($"-{myList[i]}");
			}
			for (int i = 0; i < 5 && i < myList.Count; i++)
			{
				Console.WriteLine($"+{myList[myList.Count - 1 - i]}");
			}
		}
    }
}
