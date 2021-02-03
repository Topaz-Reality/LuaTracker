using Godot;
using System;
using System.Net;
using System.Net.Sockets;

using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

public class Helpers
{
	public static IPAddress GetLocalIPAddress()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		
		foreach (var ip in host.AddressList)
			if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().Contains("192.168"))
				return ip;
		
		throw new Exception("No network adapters with an IPv4 address in the system!");
	}
}
