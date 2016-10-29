using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace KeywordExtractorServer
{
    class Program
    {
		static void Main(string[] args)
		{
			var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1234);

			listener.Start();
			while (true)
			{
				//var client = listener.AcceptTcpClient();
				var socket = listener.AcceptSocket();
				Console.WriteLine("Accepted Socket Connection");
				var bytes = new Byte[socket.Available];
				socket.Receive(bytes);
				Console.WriteLine("Received " + bytes);
			}
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
