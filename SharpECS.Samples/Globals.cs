using System;

namespace EfD2
{
	public enum EntityType
	{
		None,
		Wall,
		Floor,
		Player,
		Item
	};

	public enum MoveDirection
	{
		None,
		Up,
		Down,
		Left,
		Right
	};

	public sealed class Globals
	{
		public static Globals Instance { get; private set; }

	     //private APIClass _APIClass; 

	     private Globals()
	     {
	        //_APIClass = new APIClass();  
	     }

	     //public APIClass API { get { return _APIClass; } }

	     static Globals() { Instance = new Globals(); }     
	}
}
