using System;
using System.Linq;
using System.Net;

namespace LocalHostNameAndIp
{
	public class Program
	{
		public  static void Main()
		{
			var hostName = Dns.GetHostName();
			Console.WriteLine(hostName);

			IPHostEntry entry = Dns.GetHostEntry(hostName);
			Console.WriteLine(entry.HostName);

			foreach (var ip in entry.AddressList.Where(x=>x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
			{
				Console.WriteLine(ip);
			}
			Console.ReadLine();
		}
	}
}
