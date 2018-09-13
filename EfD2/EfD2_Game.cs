﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using ECS;
using System.Collections.Generic;
using EfD2.Systems;
using EfD2.Components;
using IndependentResolutionRendering;
using Microsoft.Xna.Framework.Media;

namespace EfD2
{
	public class EfD2_Game : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public static ContentManager gameContent;

		KeyboardState keyboard;
		KeyboardState previousKeyboard;

		MouseState mouse;
		MouseState previousMouse;

		ActorSystem actorSystem;
		CollisionSystem collisionSystem;
		DrawingSystem drawingSystem;
		EventSystem eventSystem;
		GameSystem gameSystem;
		InputSystem inputSystem;
		//LogicSystem logicSystem;
		MapSystem mapSystem;
		MovementSystem movementSystem;
		PhysicsSystem physicsSystem;
		TextSystem textSystem;
		TimeSystem timeSystem;

		public EfD2_Game()
		{
			graphics = new GraphicsDeviceManager(this);
			Resolution.Init(ref graphics);

			gameContent = Content;

			Content.RootDirectory = "Content";

			Resolution.SetVirtualResolution(256, 224);
			Resolution.SetResolution(256 * 3, 224 * 3, false);

			IsMouseVisible = true;
			graphics.SynchronizeWithVerticalRetrace = true;
			IsFixedTimeStep = false;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Systems will refresh when new Entities have compatible components added to them.
			actorSystem = new ActorSystem();
			collisionSystem = new CollisionSystem();
			drawingSystem = new DrawingSystem(ref gameContent, ref spriteBatch);
			eventSystem = new EventSystem();
			gameSystem = new GameSystem();
			inputSystem = new InputSystem();
			mapSystem = new MapSystem(ref gameContent);
			movementSystem = new MovementSystem();
			physicsSystem = new PhysicsSystem();
			textSystem = new TextSystem(ref gameContent, ref spriteBatch);
			timeSystem = new TimeSystem();
	
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			Entity playerEntity;
			Entity swordHorizontal;
			Entity swordVertical;
			Entity pileOfGold;
			Entity weapon;
			Entity someTestText;
			Entity theGame;

			playerEntity = new Entity("Player");
			swordHorizontal = new Entity("Sword Horizontal");
			swordVertical = new Entity("Sword Vertical");
			pileOfGold = new Entity("gold");
			weapon = new Entity("weapon");
			someTestText = new Entity("text");

			someTestText.AddComponent(new Positionable { CurrentPosition = new Vector2(2, 5), ZOrder = (float)DisplayLayer.Text });
			someTestText.AddComponent(new Ephemeral { PersistTime = 5.0, Repetitions = 3 });
			HasText t = new HasText();
			t.Text.Add("Is this thing on?");
			t.Text.Add("I hope it works, because a \nseries of short term\narchitectural decisions were\nmade to make it work.");
			t.Text.Add("Oh well...\nShip it!");
			t.Homgeneous = false;
			t.Border = true;
			someTestText.AddComponent(t);

			pileOfGold.AddComponent(new Positionable { CurrentPosition = new Vector2(100, 100), ZOrder = (float)DisplayLayer.Floating });
			pileOfGold.AddComponent(new Collidable());
			pileOfGold.AddComponent(new Collectible() { Value = 1 });
			pileOfGold.AddComponent(new Drawable(AnimationType.Idle, new Animation(AnimationType.Idle,
											     Content.Load<Texture2D>("chest_gold0"),
                                                 Content.Load<Texture2D>("chest_gold1"))) { ZOrder = DisplayLayer.Floating });

			weapon.AddComponent(new Positionable { CurrentPosition = new Vector2(130, 130), ZOrder = (float)DisplayLayer.Floating });
			weapon.AddComponent(new Collidable());
			weapon.AddComponent(new Drawable(new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_1"))));

			//mapSystem.GenerateMap();

			//swordHorizontal.AddComponent

			playerEntity.AddComponent(new Positionable() { ZOrder = (float)DisplayLayer.Player });
			playerEntity.AddComponent(new Movable() { MoveSpeed = 60 });
			playerEntity.AddComponent(new Collidable() { BoundingBox = new RectangleF(0, 0, 6, 7) });
			playerEntity.AddComponent(new Actor() { Type = ActorType.Player });
			playerEntity.AddComponent(new Health() { Value = 3 });
			playerEntity.AddComponent(new Attacking() { AttactState = AttackStateType.NotAttacking });
			playerEntity.AddComponent(new Inventory());

			var d = new Drawable(new Animation(AnimationType.Idle,
													   Content.Load<Texture2D>("player0")),
								 new Animation(AnimationType.Running,
													   Content.Load<Texture2D>("player0"),
													   Content.Load<Texture2D>("player1"),
													   Content.Load<Texture2D>("player0"),
					 								   Content.Load<Texture2D>("player2"))) { ZOrder = DisplayLayer.Player };
			playerEntity.AddComponent(d);
			playerEntity.AddComponent(new HasInput());

			theGame = new Entity("The Game");
			theGame.AddComponent(new GameState() { State = GameStateType.Intro } );

			//Song song = Content.Load<Song>("01 The Guillotine Factory - Assembly Line");
			Song song = Content.Load<Song>("13 H-Pizzle - Ghosts of a Fallen Empire");
			//MediaPlayer.Play(song);
			MediaPlayer.Volume = 0.2f;
		}

		protected override void UnloadContent()
		{
			Dispose();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			keyboard = Keyboard.GetState();
			mouse = Mouse.GetState();

			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif

			if (keyboard.IsKeyDown(Keys.Space) && previousKeyboard.IsKeyUp(Keys.Space))
			{
				mapSystem.GenerateMap();
				//playerEntity.GetComponent<Positionable>().CurrentPosition = mapSystem.GetOpenSpaceNearEntrance();
			}

			// these systems generate signals used by other systems
			inputSystem?.Update(gameTime);
			movementSystem?.Update(gameTime);
			collisionSystem?.Update(gameTime);
			timeSystem?.Update(gameTime);
			gameSystem?.Update(gameTime);

			// these systems do intermediate processing
			mapSystem?.Update();
			actorSystem?.Update(gameTime);

			// these systems cancel out signals generated by other systems
			eventSystem?.Update(gameTime);
			physicsSystem?.Update(gameTime);

			previousMouse = mouse;
			previousKeyboard = keyboard;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			Resolution.BeginDraw();

			graphics.GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Resolution.getTransformationMatrix());
				drawingSystem?.Animate(gameTime);
				textSystem.Update(gameTime);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
