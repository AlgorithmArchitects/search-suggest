using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace KeywordExtractorServer
{
    class Program
    {
		static void Main(string[] args)
		{
			var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1234);
			Console.WriteLine("Starting Listener...");

			while (true)
			{
				listener.Start();
				//var client = listener.AcceptTcpClient();
				var socket = listener.AcceptSocket();
				Console.WriteLine("Accepted Socket Connection");

				var command = "";
				while (command.Count() == 0 || command.Last() != '\n')
				{
					var bytes = new Byte[socket.Available];
					socket.Receive(bytes, socket.Available, 0);
					command += Encoding.UTF8.GetString(bytes);
				}
				Console.WriteLine("Received " + command);

				if (new Regex("^GET").IsMatch(command))
				{
					Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
						+ "Connection: Upgrade" + Environment.NewLine
						+ "Upgrade: websocket" + Environment.NewLine
						+ "Sec-WebSocket-Accept: " + Convert.ToBase64String(
							SHA1.Create().ComputeHash(
								Encoding.UTF8.GetBytes(
									new Regex("Sec-WebSocket-Key: (.*)").Match(command).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
								)
							)
						) + Environment.NewLine
						+ Environment.NewLine);

					var responseString = Encoding.UTF8.GetString(response);
					socket.Send(response, response.Length, 0);
					socket.Close();
				}
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
