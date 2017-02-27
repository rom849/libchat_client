using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace libchat2
{
	public class MyClass
	{
		public Socket sock;
		public MyClass (String nick, String IP, int PORT)
		{
			IPAddress host;
			try{
				 host = IPAddress.Parse(IP);
			}
			catch{
				//Console.WriteLine (Dns.GetHostEntry (IP).AddressList[0].ToString ());
				host = IPAddress.Parse(Dns.GetHostEntry(IP).AddressList[0].ToString());
			}
			IPEndPoint hostep = new IPEndPoint(host, PORT);
			sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				sock.Connect(hostep);

			} catch (SocketException e) {
				//Console.WriteLine("Problem connecting to host");
				//Console.WriteLine(e.ToString());

				sock.Close();
				throw e;
				return;
			}

			try
			{
				sock.Send(Encoding.ASCII.GetBytes("nick$"+nick));
			} catch (SocketException e) {
				//Console.WriteLine("Problem sending data");
				//Console.WriteLine( e.ToString());
				sock.Close();
				throw e;
				return;
			}


		}

		public void send(string msg)
		{
			try
			{
				List<byte> aa = new List<byte>();
				aa.Add( (byte)Encoding.ASCII.GetBytes(msg).Length);
				aa.AddRange( Encoding.ASCII.GetBytes(msg));
				sock.Send(aa.ToArray());
			} catch (SocketException e) {
				//Console.WriteLine("Problem sending data");
				//Console.WriteLine( e.ToString());
				throw e;
				sock.Close();
				return;
			}
		}

		private bool TryReadExact(Stream stream, byte[] buffer, int offset, int count, Socket tcpClient)
		{
			int bytesRead;

			while (tcpClient.Connected && count > 0 && ((bytesRead = stream.Read(buffer, offset, count)) > 0))
			{
				offset += bytesRead;
				count -= bytesRead;
			}

			return count == 0;
		}
		private string ReplaceCommonEscapeSequences(string s)
		{
			return s.Replace("\\n", "\n").Replace("\\r", "\r");
		}
		byte[] bytes = new byte[1024];  
		public string Receive()
		{
			const int totalByteBuffer = 4096;
			byte[] buffer = new byte[256];


			using (var networkStream = new NetworkStream(sock))//tcpClient.GetStream())
				using (var bufferedStream = new BufferedStream(networkStream, totalByteBuffer))
					while (true)
			{
				try
				{

					try
					{
						//Console.WriteLine("1");
						// Create a file to write to.



						// Receive header - byte length.
						if (sock.Connected)
						{
							if (!TryReadExact(bufferedStream, buffer, 0, 1, sock))
							{
								//Console.WriteLine("noread");
								break;
							}
						}
						else 
						{
							//Console.WriteLine("noconn");
							break;
						}
						byte messageLen = buffer[0];
						//Console.WriteLine(messageLen.ToString());
						//this.Invoke((MethodInvoker)delegate { this.boxRemote.Text += messageLen + "\n"; });
						// Receive the ASCII bytes.
						if (sock.Connected)
							if (!TryReadExact(bufferedStream, buffer, 1, messageLen,sock))
						{
							//Console.WriteLine("noread2");
							break;
						}
						//this.Invoke((MethodInvoker)delegate { this.boxRemote.Text += "recv" + "\n"; });

						var message = Encoding.ASCII.GetString(buffer, 1, messageLen);
						//MessageBox.Show("Client", "Message received: " + message);
						string textToWrite = ReplaceCommonEscapeSequences(message);
						return textToWrite;
					}
					catch(Exception e){throw e;Console.WriteLine(e.Message);}
				}
				catch(Exception e){throw e; Console.WriteLine(e.Message);}

			}

			return "no";
		}

	}
}
