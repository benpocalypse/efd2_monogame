using Microsoft.Xna.Framework;
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

		TextSystem textSystem;
		InputSystem inputSystem;
		CollisionSystem collisionSystem;
		PhysicsSystem physicsSystem;
		DrawingSystem drawingSystem;
		MapSystem mapSystem;
		GameSystem gameSystem;

		Entity playerEntity;
		Entity pileOfGold;

		Entity someTestText;

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
			// Systems will refresh when new Entities have compatible components added to them.
			collisionSystem = new CollisionSystem();

			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			drawingSystem = new DrawingSystem(ref gameContent, ref spriteBatch);
			textSystem = new TextSystem(ref gameContent, ref spriteBatch);
			inputSystem = new InputSystem();

			physicsSystem = new PhysicsSystem();
			mapSystem = new MapSystem(ref gameContent);

			playerEntity = new Entity("Player");
			pileOfGold = new Entity("gold");
			someTestText = new Entity("text");

			someTestText.AddComponent(new Positionable { CurrentPosition = new Vector2(5, 5), ZOrder = 1.0f });
			someTestText.AddComponent(new Ephemeral { PersistTime = 5.0, TotalTime = 5.0f });
			HasText t = new HasText();
			t.Text.Add("Is this thing on?\nI really do sure hope so!");
			t.Text.Add("If it works, this can be a\ncool feature to use.");
			t.Border = true;
			someTestText.AddComponent(t);

			pileOfGold.AddComponent(new Positionable { CurrentPosition = new Vector2(100, 100), ZOrder = 0.9f });
			pileOfGold.AddComponent(new Collidable { Type = EntityType.Item });
			pileOfGold.AddComponent(new Collectible { Type = CollectibleType.Gold });
			pileOfGold.AddComponent(new Drawable(AnimationType.Idle, new Animation(AnimationType.Idle,
											     Content.Load<Texture2D>("chest_gold0"),
                                                 Content.Load<Texture2D>("chest_gold1"))));

			mapSystem.GenerateMap();

			playerEntity.AddComponent(new Positionable() { CurrentPosition = mapSystem.GetOpenSpaceNearEntrance(), ZOrder = 0.9f });
			playerEntity.AddComponent(new Movable() { MoveSpeed = 60 });
			playerEntity.AddComponent(new Collidable() { Type = EntityType.Player, BoundingBox = new RectangleF(0, 0, 6, 7) });
			playerEntity.AddComponent(new PlayerStatable() { PlayerState = PlayerStateType.None });

			var d = new Drawable(new Animation(AnimationType.Idle,
													   Content.Load<Texture2D>("player0")),
								 new Animation(AnimationType.Running,
													   Content.Load<Texture2D>("player0"),
													   Content.Load<Texture2D>("player1"),
													   Content.Load<Texture2D>("player0"),
					 								   Content.Load<Texture2D>("player2")));
			d.ZOrder = DisplayLayer.Player;
			playerEntity.AddComponent(d);

			//Song song = Content.Load<Song>("01 The Guillotine Factory - Assembly Line");
			Song song = Content.Load<Song>("13 H-Pizzle - Ghosts of a Fallen Empire");
			MediaPlayer.Play(song);
			MediaPlayer.Volume = 0.2f;
	
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{

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
				playerEntity.GetComponent<Positionable>().CurrentPosition = mapSystem.GetOpenSpaceNearEntrance();
			}

			inputSystem?.Update(gameTime);
			collisionSystem?.Update(gameTime);
			physicsSystem?.Update(gameTime);
			mapSystem?.Update();

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
