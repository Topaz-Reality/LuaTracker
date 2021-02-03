using Godot;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class TCPClass
{
	public static List<string> _catList = new List<string>()
	{
		"MAGIC",
		"FORMS",
		"CHARMS",
		"PROOFS",
		"MISC",
		"ABILITIES"
	};
	
	public static List<int> _catAmount = new List<int>()
	{
		3,
		7,
		7,
		1,
		13,
		4
	};
	
	public static Dictionary<int, int> _itemList = new Dictionary<int, int>()
	{
		{0x032F0C4, 0},
		{0x032F0C5, 0},
		{0x032F0C6, 0},
		{0x032F0C7, 0},
		{0x032F0FF, 0},
		{0x032F100, 0},
		
		{0x032EE26, 1},
		{0x032EE5E, 1},
		{0x032EE96, 1},
		{0x032EECE, 1},
		{0x032EF06, 1},
		
		{0x000FFFC, 2},
		{0x000FFFD, 2},
		{0x000FFFE, 2},
		{0x000FFFF, 2},
		{0x032F056, 2},
		
		{0x032F1E2, 3},
		{0x032F1E3, 3},
		{0x032F1E4, 3},
		
		{0x032F1F4, 4},
		{0x032F1C4, 4},
		{0x032C8C0, 4},
		{0x032E02F, 4},
		
		{0x032E0FE, 5},
		{0x032E100, 5},
		{0x032E102, 5},
		{0x032E104, 5},
		{0x032E106, 5}
	};
	
	public static string HandleAPI(string Input, MainServer MainNode)
	{
		int _lenAPI = (Input.IndexOf(']') - 1) - (Input.IndexOf('[') + 1);
		
		if (_lenAPI < 0)
			return "[ERROR] - Invalid APICall Format.";
		
		var _strAPI = Input.Substring(Input.IndexOf('[') + 1, _lenAPI + 1);
		
		if (!_strAPI.Contains("APICall::"))
			return "[ERROR] - Not an APICall.";
		
		var _extStr = Input.Substring(Input.IndexOf('(') + 1);
		_extStr = _extStr.Remove(_extStr.Length - 1);
		
		switch (_strAPI)
		{
			case "APICall::ITEM":
			case "APICall::FORMLVL":
			case "APICall::LEVEL":
			case "APICall::ABILITY":
			{
				var _splStr = _extStr.Split(" | ");
				
				var _key = Convert.ToInt32(_splStr[0], 16);
				var _value = Convert.ToInt32(_splStr[1], 16);
				
				var _itemCat = _itemList[_key];
				
				var _target = MainNode.GetNode("CONTROLS/" + _catList[_itemCat] + "/0x" + _key.ToString("X7")) as TextureRect;
				
				var _number = _target.GetNode("_amount") as TextureRect;
				var _animEngine = _target.GetNode("_player") as AnimationPlayer;
				
				if (_strAPI == "APICall::ABILITY" && _target.RectPosition.y != 305)
				{
					_animEngine.Play("_animIn");
					while (_animEngine.IsPlaying());
				}
				
				if (_value == 1 && _strAPI != "APICall::LEVEL")
				{
					if (_number != null)
					{
						if (_number.RectPosition.y < 60)
						{
							var _anim = _animEngine.GetAnimation("_animSubt");
							
							_anim.TrackInsertKey(0, 0.0F, _target.SelfModulate);
							_anim.TrackInsertKey(1, 0.0F, _number.RectPosition);
							_anim.TrackInsertKey(2, 0.0F, _number.SelfModulate);
							
							_animEngine.Play("_animSubt");
						}
						
						else
							_animEngine.Play("_animIn");
					}
					
					else
						_animEngine.Play("_animIn");
				}
				
				else if (_value > 1 || _strAPI == "APICall::LEVEL")
				{
					var _anim = _animEngine.GetAnimation("_animChange");
					var _tex = ResourceLoader.Load(string.Format("res://Assets/Numbers/{0}.png", _value)) as Texture;
					
					if (_value > 15 && _value < 99)
						_tex = ResourceLoader.Load(string.Format("res://Assets/Numbers/{0}.png", ((_value - 15) / 5) + 10)) as Texture;
					
					else if (_value >= _catAmount[_itemCat] || _value == 99)
						_tex = ResourceLoader.Load("res://Assets/Numbers/max.png") as Texture;
					
					_anim.TrackInsertKey(0, 0.0F, _target.SelfModulate);
					
					if (_number != null)
					{
						_anim.TrackInsertKey(1, 0.0F, _number.RectPosition);
						_anim.TrackInsertKey(2, 0.0F, _number.SelfModulate);
						_anim.TrackInsertKey(3, 0.2F, _tex);
					}
					_animEngine.Play("_animChange");
				}
				
				if (_strAPI == "APICall::LEVEL")
					return string.Format("[APICALL] - Level set to {0}", _splStr[1]) + "\n";
				else
					return string.Format("[APICALL] - Register {0} of {1}.", _splStr[1], _splStr[0]) + "\n";
			}
			
			case "APICall::REMOVE":
			{
				var _key = Convert.ToInt32(_extStr, 16);
				
				var _itemCat = _itemList[_key];
				var _target = MainNode.GetNode("CONTROLS/" + _catList[_itemCat] + "/0x" + _key.ToString("X7")) as TextureRect;
				
				var _number = _target.GetNode("_amount") as TextureRect;
				var _animEngine = _target.GetNode("_player") as AnimationPlayer;
				var _anim = _animEngine.GetAnimation("_animOut");
				
				_anim.TrackInsertKey(0, 0.0F, _target.SelfModulate);
				
				if (_number != null)
				{
					_anim.TrackInsertKey(1, 0.0F, _number.RectPosition);
					_anim.TrackInsertKey(2, 0.0F, _number.SelfModulate);
				}
				
				_animEngine.Play("_animOut");
				
				return string.Format("[APICALL] - Remove Item {0}.", _extStr) + "\n";
			}
			
			case "APICall::FORMGET":
			{
				int _key = Convert.ToInt32(_extStr, 16);
				
				var _itemCat = _itemList[_key];
				
				var _target = MainNode.GetNode("CONTROLS/" + _catList[_itemCat] + "/0x" + _key.ToString("X7")) as TextureRect;
				var _animEngine = _target.GetNode("_player") as AnimationPlayer;
				
				_animEngine.Play("_animIn");
				
				return string.Format("[APICALL] - Obtained form {0}.", _extStr) + "\n";
			}
			
			case "APICall::MESSAGE":
				return string.Format("[CLIENT] - {0}", _extStr) + "\n";
			
			default:
				return "[ERROR] - APICall not defined.";
		}
	}
}
