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
		EnterMap,
		Playing
	};

	public enum GameEventType
	{
		None,
		PlayerHitExit,
		EndOfLevel
	};

	public enum EventTrigger
	{
		None,
		Collision,
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

    public enum ActorStateType
    {
        None,
        Idle,
        Moving,
        Attacking,
        Hurt
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


    // Thread safe Singleton that uses Lazy loading, via - http://csharpindepth.com/articles/general/singleton.aspx
    public sealed class Globals
    {
        public const float Acceleration = 0.05f;
        public const float Decceleration = 0.1f;

        private static readonly Lazy<Globals> lazy =
            new Lazy<Globals>(() => new Globals());

        public static Globals Instance { get { return lazy.Value; } }

        private Globals()
        {
        }
    }
}
