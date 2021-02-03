using Godot;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;

using SThread = System.Threading.Thread;

public class MainServer : Control
{
	TcpListener _godotServer;
	TcpClient _luaClient;
	NetworkStream _luaStream;
	
	SThread _threadOutput;
	SThread _threadCheck;
	
	bool _buttonStatus = false;
	string _message = "";
	
	public ColorRect Container;
	public RichTextLabel Console;
	
	public void CHKThread()
	{
		while (true)
		{
			if (_luaStream != null)
			{
				var _getData = new byte[_luaClient.ReceiveBufferSize];
				var _length = _luaStream.Read(_getData, 0, _getData.Length);
				
				if (_length == 0)
				{
					_luaStream = null;
					
					if (Container.Visible)
						Console.Text = "[SERVER] - Lost connection from client!\n";
				}
			}
			
			SThread.Sleep(500);
		}
	}
	
	public void TCPThread()
	{
		while (true)
		{
			try
			{
				if (_luaStream == null)
					_luaStream = _luaClient.GetStream();
					
				else if (_luaStream.DataAvailable)
				{
					var _getData = new byte[_luaClient.ReceiveBufferSize];
					var _length = _luaStream.Read(_getData, 0, _getData.Length);
					
					_message = Encoding.ASCII.GetString(_getData, 0, _length);
				}
				
				else if (_message != "")
				{
					var _list = _message.Split("~");
					var _console = "";
					
					foreach (var a in _list)
					{
						var _str = TCPClass.HandleAPI(a, this);
						
						if (Container.Visible)
							Console.Text += _str;
						
						SThread.Sleep(50);
					}
					
					_message = "";
				}
			}
			
			catch (System.NullReferenceException) { _luaStream = _luaClient.GetStream(); }
		}
	}
	
	private void PressButton()
	{
		var _sender = GetNode("BUTTON") as Button;
		
		Container.Visible = !Container.Visible;
		_buttonStatus = !_buttonStatus;
		
		if (!_buttonStatus)
			_sender.Text = "Hide the Console";
		
		else
			_sender.Text = "Show the Console";
	}
	
	public override void _Ready()
	{
		Container = GetNode("CONSOLE") as ColorRect;
		Console = GetNode("CONSOLE/_text") as RichTextLabel;
		
		var _ipLabel = GetNode("ADDRESS") as RichTextLabel;
		IPAddress _localhost = Helpers.GetLocalIPAddress();
		
		_godotServer = new TcpListener(_localhost, 1392);  
		_godotServer.Start();
		
		_ipLabel.BbcodeText = string.Format("[center]IP Address: {0}[/center]", _localhost.ToString());
		
		Console.Text += string.Format("[SERVER] - Secured a host connection over tpc://{0}:1392/" + "\n", _localhost.ToString());
		Console.Text += "[SERVER] - Currently listening to PORT 1392..." + "\n";
		
		OS.SetWindowTitle("LuaTracker [v0.10 - BETA]");
	}
	
	public override async void _Process(float delta)
	{
		if (_luaClient == null)
			_luaClient = await _godotServer.AcceptTcpClientAsync();
		
		else if (_threadOutput == null)
		{
			_threadOutput = new System.Threading.Thread(new ThreadStart(this.TCPThread));
			_threadOutput.IsBackground = false;
			_threadOutput.Start();
			
			_threadCheck = new System.Threading.Thread(new ThreadStart(this.CHKThread));
			_threadCheck.IsBackground = false;
			_threadCheck.Start();
		}
	}
}
