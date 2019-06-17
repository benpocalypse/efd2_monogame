using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EfD2
{
    public enum GameStateType
	{
		Intro,
		TitleScreen,
		Playing,
        GameOver,
        Unknown
	};

    // This is used to inform across Systems.
	public enum GameEventType
	{
		None,
        EnteredLevel,
		ExitedLevel
	};

	public enum EventTrigger
	{
		None,
        GameState,
		Collision,
        Action,
		Time
	};

	public enum ActorType
	{
		None,
		Player,
		Monster,
		Entrance,
		Exit
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
		Moving,
		Attacking,
		Hurt
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

	public enum InputValue
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


    // Thread safe Singleton that uses Lazy loading, via - http://csharpindepth.com/articles/general/singleton.aspx
    /*
    public sealed class Globals
    {
        public const float Acceleration = 0.05f;
        public const float Decceleration = 0.1f;
        public const string PlayerString = "The Player";

        private static readonly Lazy<Globals> lazy =
            new Lazy<Globals>(() => new Globals());

        public static Globals Instance { get { return lazy.Value; } }

        private Globals()
        {
        }
    }
    */

    public sealed class Globals
    {
        private static readonly Globals instance = new Globals();

        public readonly float Acceleration = 0.05f;
        public readonly float Decceleration = 0.1f;
        public readonly string PlayerTitle = "ThePlayer";
        public readonly string OpenSpaceNearEntranceTitle = "OpenSpaceNextToEntrance";
        public readonly string GameTitle = "TheGame";
        public readonly string LevelEntranceTitle = "LevelEntrance";
        public readonly string LevelExitTitle = "LevelExit";

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Globals()
        {
        }

        private Globals()
        {
        }

        public static Globals Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
