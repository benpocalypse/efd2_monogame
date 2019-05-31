using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using ECS;
using System.Collections.Generic;
using EfD2.Systems;
using EfD2.Systems.SupportSystems;
using EfD2.Components;
using IndependentResolutionRendering;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using EfD2.Helpers;

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

		CollisionSystem collisionSystem;
		DrawingSystem drawingSystem;
        LogicSystem logicSystem;
		HudSystem hudSystem;
		InputSystem inputSystem;
		MapSystem mapSystem;
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
			collisionSystem = new CollisionSystem();
			drawingSystem = new DrawingSystem(ref gameContent, ref spriteBatch);
			logicSystem = new LogicSystem();
			hudSystem = new HudSystem(ref gameContent, ref spriteBatch);
			inputSystem = new InputSystem();
			mapSystem = new MapSystem(ref gameContent);
			physicsSystem = new PhysicsSystem();
			textSystem = new TextSystem(ref gameContent, ref spriteBatch);
			timeSystem = new TimeSystem();
	
			base.Initialize();
		}

        private Entity fpsText;

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
            Entity sword;
			Entity someTestText;
			Entity theGame;

            var g = Globals.Instance;

            playerEntity = new Entity(g.PlayerTitle);
			swordHorizontal = new Entity("Sword Horizontal");
			swordVertical = new Entity("Sword Vertical");
			pileOfGold = new Entity("gold");
			sword = new Entity("sword");
			someTestText = new Entity("text");

			someTestText.AddComponent(new Positionable { CurrentPosition = new Vector2(4, 7), ZOrder = (float)DisplayLayer.Text });
			someTestText.AddComponent(new Ephemeral { PersistTime = 5.0, Repetitions = 3 });
			Text t = new Text();
			t.TextList.Add("Is this thing on?");
			t.TextList.Add("I hope it works, because a \nseries of short term\narchitectural decisions were\nmade to make it work.");
			t.TextList.Add("Oh well...\nShip it!");
			t.Homgeneous = false;
			t.Border = true;
			someTestText.AddComponent(t);

            fpsText = new Entity("Frames Per Second");
            fpsText.AddComponent(new Positionable { CurrentPosition = new Vector2(20, 2), ZOrder = (float)DisplayLayer.Text });

            pileOfGold.AddComponent(new Positionable { CurrentPosition = new Vector2(100, 100), ZOrder = (float)DisplayLayer.Floating });
			pileOfGold.AddComponent(new Collidable());
			pileOfGold.AddComponent(new Collectible() { Type = CollectibleType.Gold, Value = 1 });
			pileOfGold.AddComponent(new Drawable(AnimationType.Idle, new Animation(AnimationType.Idle,
											     Content.Load<Texture2D>("chest_gold0"),
                                                 Content.Load<Texture2D>("chest_gold1"))) { ZOrder = DisplayLayer.Floating });

			sword.AddComponent(new Positionable { CurrentPosition = new Vector2(130, 130), ZOrder = (float)DisplayLayer.Floating });
            sword.AddComponent(new Collectible());
            sword.AddComponent(new Collidable());
            sword.AddComponent(new Damage() { Value = 1 });
            sword.AddComponent(new Ephemeral() { PersistTime = 1, Active = false });
            sword.AddComponent(new Drawable(new Animation(AnimationType.None, Content.Load<Texture2D>("sword1"))) { Visible = false});

			playerEntity.AddComponent(new Positionable() { ZOrder = (float)DisplayLayer.Player });
			playerEntity.AddComponent(new Movable() { MoveSpeed = 60 });
			playerEntity.AddComponent(new Collidable() { BoundingBox = new RectangleF(0, 0, 6, 6) });
			playerEntity.AddComponent(new Actor() { Type = ActorType.Player });
			playerEntity.AddComponent(new Attacking() { AttactState = AttackStateType.NotAttacking });
			playerEntity.AddComponent(new Inventory());
			playerEntity.AddComponent(new Health() { Max = 3, Value = 2 });
            playerEntity.AddComponent(new Equipment() { Item = sword });

			var d = new Drawable(new Animation(AnimationType.Idle,
													   Content.Load<Texture2D>("player0")),
								 new Animation(AnimationType.Moving,
													   Content.Load<Texture2D>("player0"),
													   Content.Load<Texture2D>("player1"),
													   Content.Load<Texture2D>("player0"),
					 								   Content.Load<Texture2D>("player2")),
                                 new Animation(AnimationType.Attacking,
                                                        Content.Load<Texture2D>("player1"))) { ZOrder = DisplayLayer.Player };
			playerEntity.AddComponent(d);
			playerEntity.AddComponent(new Input());

			theGame = new Entity("The Game");
			theGame.AddComponent(new GameState() { CurrentState = GameStateType.Intro } );
            theGame.AddComponent(new Events());

			//Song song = Content.Load<Song>("01 The Guillotine Factory - Assembly Line");
			Song song = Content.Load<Song>("13 H-Pizzle - Ghosts of a Fallen Empire");
			//MediaPlayer.Play(song);
			MediaPlayer.Volume = 0.2f;
		}

		protected override void UnloadContent()
		{
			Dispose();
		}

        private Stopwatch stopWatch = new Stopwatch();
        float FPS = 0.0f;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
		{

            stopWatch.Start();

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
            mapSystem?.Update();
            logicSystem?.Update(gameTime);
            inputSystem?.Update(gameTime);
			collisionSystem?.Update(gameTime);
			timeSystem?.Update(gameTime);
			

			// these systems do intermediate processing
			

			// these systems cancel out signals generated by other systems
			physicsSystem?.Update(gameTime);

			previousMouse = mouse;
			previousKeyboard = keyboard;

			base.Update(gameTime);

            stopWatch.Stop();
            FPS = 1000f / (float)stopWatch.Elapsed.Milliseconds;

            fpsText.RemoveComponents<Text>();

            Text t = new Text();
            t.TextList.Add("FPS: " + FPS.ToString("00.00"));
            t.Homgeneous = false;
            t.Border = false;
            fpsText.AddComponent(t);

            stopWatch.Reset();
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
				drawingSystem?.Update(gameTime);
				textSystem?.Update(gameTime);
				hudSystem?.Update(gameTime);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
