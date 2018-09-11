using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EfD2
{
	// FIXME - Remove this enum
	public enum EntityType
	{
		None,
		Wall,
		Floor,
		Player,
		Item,
		Weapon,
		Gold,
		Health,
		Entrance,
		Exit
	};

	/* FIXME - Notes
	 * I can't decide how to handle hitting the map exit, and what the PhysicsSystem should be responsible for?
	 * My initial thoughts were to have the PhysicsSystem only move things that needed to be moved, and mark
	 * them as not colliding. However, how do you handle the player running into the Map Exit? How about entities
	 * that should have a reaction such as a player getting hit, or an Enemy being killed? Fuck.
	 */ 

	// Used to prompt a reaction from an event
	public enum EventType
	{
		None,
		Exit
	};

	public enum EventTrigger
	{
		None,
		Collision
	};

	public enum ActorStateType
	{
		None,
		Immune,
		Hurt,
		Dead,
		HitExit
	};

	public enum AttackStateType
	{
		Attacking,
		NotAttacking
	};

	public enum CollectibleType
	{
		None,
		Gold,
		Health
	};

	public enum Direction
	{
		None,
		Up,
		Down,
		Left,
		Right
	};

	public enum AnimationType
	{
		None,
		Idle,
		Running,
		Attacking,
		Hurt
	};

	public enum GameStateType
	{
		Intro,
		TitleScreen,
		Playing
	};

	public enum Visibility
	{
		Visible,
		Hidden
	};

	public enum DisplayLayer
	{
		Background,
		Player,
		Particles,
		Foreground,
		Floating,
		TextBackground,
		Text,
		Menu,
		MAX_LAYER
	};

	public enum Input
	{
		None,
		Up,
		Down,
		Left,
		Right,
		A,
		B,
		Select,
		Start
	};

	public enum InputType
	{
		Direct, // Moves 1:1 with the input given
		Menu
	};

	public class Animation : IComparable<Animation>
	{
		public AnimationType Type { get; set; } = AnimationType.None;
		public List<Texture2D> FrameList { get; set; }
		public int CurrentFrame { get; set; } = 0;
		public float FrameSpeed { get; set; } = 0.1f; // In seconds per frame
		public float FrameCounter { get; set; } = 0.0f;

		public Animation(params Texture2D[] textureNames)
		{
			FrameList = new List<Texture2D>();

			foreach(Texture2D t in textureNames)
			{
				FrameList.Add(t);
			}
		}

		public Animation(AnimationType aType, params Texture2D[] textureNames) : this(textureNames)
		{
			Type = aType;
		}

		public int CompareTo(Animation other)
		{
			return this.Type.CompareTo(other.Type);
		}
	}

	public sealed class Globals
	{
		public static Globals Instance { get; private set; }

		public const float Acceleration = 0.05f;
		public const float Decceleration = 0.1f;

		private Globals()
		{
			//_APIClass = new APIClass();  
		}

	     //public APIClass API { get { return _APIClass; } }

		static Globals() 
		{ 
			Instance = new Globals(); 
		}
	}

	public class RectangleF
	{
		protected float _x = 0.0F;
		protected float _y = 0.0F;
		protected float _width = 0.0F;
		protected float _height = 0.0F;
		protected float _x2 = 0.0F;
		protected float _y2 = 0.0F;

		public RectangleF()
		{

		}

		public Rectangle ToRectangle()
		{
			Rectangle myReturn = new Rectangle((int)_x, (int)_y, (int)_width, (int)_height);
			return myReturn;
		}

		public RectangleF(float pX, float pY, float pWidth, float pHeight)
		{
			_x = pX;
			_y = pY;
			_width = pWidth;
			_height = pHeight;
			_x2 = pX + pWidth;
			_y2 = pY + pHeight;
		}

		public bool Contains(Vector2 pPoint)
		{
			if ((pPoint.X > this._x) && (pPoint.X < this._x2) && (pPoint.Y > this._y) && (pPoint.Y < this._y2))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public RectangleF Union(RectangleF rect1, RectangleF rect2)
		{
			RectangleF tempRect = new RectangleF();

			if (rect1._x < rect2._x)
			{
				tempRect._x = rect1._x;
			}
			else
			{
				tempRect._x = rect2._x;
			}

			if (rect1._x2 > rect2._x2)
			{
				tempRect._x2 = rect1._x2;
			}
			else
			{
				tempRect._x2 = rect2._x2;
			}

			tempRect._width = tempRect._x2 - tempRect._x;


			if (rect1._y < rect2._y)
			{
				tempRect._y = rect1._y;
			}
			else
			{
				tempRect._y = rect2._y;
			}

			if (rect1._y2 > rect2._y2)
			{
				tempRect._y2 = rect1._y2;
			}
			else
			{
				tempRect._y2 = rect2._y2;
			}

			tempRect._height = tempRect._y2 - tempRect._y;
			return tempRect;
		}
		public float X
		{
			get { return _x; }
			set
			{
				_x = value;
				_x2 = _x + _width;
			}
		}

		public float Y
		{
			get { return _y; }
			set
			{
				_y = value;
				_y2 = _y + _height;
			}
		}

		public float Width
		{
			get { return _width; }
			set
			{
				_width = value;
				_x2 = _x + _width;
			}
		}

		public float Height
		{
			get { return _height; }
			set
			{
				_height = value;
				_y2 = _y + _height;
			}
		}

		public float X2
		{
			get { return _x2; }
		}

		public float Y2
		{
			get { return _y2; }
		}

		public RectangleF Duplicate()
		{
			RectangleF myReturn = new RectangleF(X, Y, Width, Height);
			return myReturn;
		}

		public bool Intersects(RectangleF r2)//(RectangleF r2, RectangleF r1)
		{
			bool myReturn = false;

			//if ((X + Width >= r2.X && Y + Height >= r2.Y && X <= r2.X + r2.Width && Y <= r2.Y + r2.Height))
			if ((X + Width > r2.X && Y + Height > r2.Y && X < r2.X + r2.Width && Y < r2.Y + r2.Height))
			{
				myReturn = true;
			}

			return myReturn;
		}
	}
}
