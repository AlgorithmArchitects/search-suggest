﻿using System;
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
					while (socket.Available == 0) ;
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

					command = "";
					var commandBytes = new LinkedList<byte>();
					while (command.Count() == 0)
					{
						while (socket.Available == 0) ;
						var bytes = new Byte[socket.Available];
						socket.Receive(bytes, socket.Available, 0);
						foreach(var b in bytes)
						{
							commandBytes.AddLast(b);
						}
						command = GetDecodedData(bytes);
					}
					Console.WriteLine("Received " + command);

					//extract the data from the command
					var usedKeywords = new LinkedList<string>();

					var index = command.IndexOf("searchString: \"");
					if (index != -1)
					{
						command = command.Substring(index + 15);
						index = command.IndexOf('\"');
						var searchString = command.Substring(0, index);
						command = command.Substring(index + 1);

						var searchStrings = searchString.Split(' ');
						foreach(var term in searchStrings)
						{
							usedKeywords.AddFirst(term);
						}
					}

					var websites = new Dictionary<string, double>();
					int maxWeight = 0;
					int minWeight = 0;

					while (command.Length > 0)
					{
						index = command.IndexOf("url: \"");
						if (index == -1)
							break;
						command = command.Substring(index + 6);
						index = command.IndexOf('\"');
						var site = command.Substring(0, index);
						command = command.Substring(index + 1);

						index = command.IndexOf("value: ");
						if (index == -1)
							break;
						command = command.Substring(index + 7);
						index = command.IndexOf('}');
						var weight = int.Parse(command.Substring(0, index));
						if (weight < minWeight)
							minWeight = weight;
						if (weight > maxWeight)
							maxWeight = weight;
						websites.Add(site, weight);
					}
					minWeight *= -1;
					var keys = websites.Keys.ToList();
					foreach(var site in keys)
					{
						if (websites[site] < 0 && minWeight > 0)
							websites[site] /= minWeight;
						else if(maxWeight > 0)
							websites[site] /= maxWeight;
					}

					var results = KeywordExtractor.ExtractKeywords(websites, usedKeywords).ToList();
					results.Sort((x, y) => x.Value.CompareTo(y.Value));

					var message = "";
					for(int i = 0;i < 3 && i < results.Count; i++)
					{
						message += results[i].Key + ',' + results[i].Value + ',';
					}
					for(int i = results.Count - 1; i > results.Count - 3 && i > 2; i--)
					{
						message += results[i].Key + ',' + results[i].Value + ',';
					}

					socket.Send(Encoding.ASCII.GetBytes(message));

					socket.Close();
				}
			}
		}

		public static string GetDecodedData(byte[] buffer)
		{
			byte b = buffer[1];
			int dataLength = 0;
			int totalLength = 0;
			int keyIndex = 0;

			if (b - 128 <= 125)
			{
				dataLength = b - 128;
				keyIndex = 2;
				totalLength = dataLength + 6;
			}
			else if (b - 128 == 126)
			{
				dataLength = BitConverter.ToInt16(new byte[] { buffer[3], buffer[2] }, 0);
				keyIndex = 4;
				totalLength = dataLength + 8;
			}
			else if (b - 128 == 127)
			{
				dataLength = (int)BitConverter.ToInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
				keyIndex = 10;
				totalLength = dataLength + 14;
			}

			byte[] key = new byte[] { buffer[keyIndex], buffer[keyIndex + 1], buffer[keyIndex + 2], buffer[keyIndex + 3] };

			int dataIndex = keyIndex + 4;
			int count = 0;
			for (int i = dataIndex; i < totalLength; i++)
			{
				buffer[i] = (byte)(buffer[i] ^ key[count % 4]);
				count++;
			}

			return Encoding.ASCII.GetString(buffer, dataIndex, dataLength);
		}
	}
}
