using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SharpECS;
using EfD2.Components;
using EfD2.Systems;
using System.Collections.Generic;

using IndependentResolutionRendering;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace EfD2.Samples
{
    internal class Game 
        : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

		public static ContentManager gameContent;

        KeyboardState keyboard;
        KeyboardState previousKeyboard;

        MouseState mouse;
        MouseState previousMouse;

        EntityPool entityPool;

		InputSystem inputSystem;
		CollisionSystem collisionSystem;
		PhysicsSystem physicsSystem;
		DrawingSystem drawingSystem;
		MapSystem mapSystem;

        Entity playerEntity;
		Entity pileOfGold;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
			Resolution.Init(ref graphics);

			gameContent = Content;

            Content.RootDirectory = "Content";

			Resolution.SetVirtualResolution(256, 224);
			Resolution.SetResolution(256*3, 224*3, false);

            IsMouseVisible = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
        }

		protected override void Initialize()
		{
			entityPool = EntityPool.New("EntityPool");

			// Systems will refresh when new Entities have compatible components added to them.
			drawingSystem = new DrawingSystem(entityPool);
			inputSystem = new InputSystem(entityPool);
			collisionSystem = new CollisionSystem(entityPool);
			physicsSystem = new PhysicsSystem(entityPool);
			drawingSystem = new DrawingSystem(entityPool);
			mapSystem = new MapSystem(entityPool, ref gameContent);

			playerEntity = entityPool.CreateEntity("Player");
			pileOfGold = entityPool.CreateEntity("gold");

			pileOfGold += new Positionable { CurrentPosition = new Vector2(100, 100), ZOrder = 1.0f };
			pileOfGold += new Collidable { Type = EntityType.Item };
			pileOfGold += new Collectible { Type = CollectibleType.Gold };
			pileOfGold += new Drawable(AnimationType.Idle, new Animation(AnimationType.Idle,
													 Content.Load<Texture2D>("chest_gold0"),
													 Content.Load<Texture2D>("chest_gold1")));

			mapSystem.GenerateMap();
			var pos = entityPool.GetEntity("Entrance").GetComponent<Positionable>();
			var col = entityPool.GetEntity("Entrance").GetComponent<Collidable>();

			playerEntity += new Positionable() { CurrentPosition = mapSystem.GetOpenSpaceNearEntrance(), ZOrder = 1.0f };
			playerEntity += new Movable() { MoveSpeed = 75 };
			playerEntity += new Collidable() { Type = EntityType.Player, BoundingBox = new RectangleF(0, 0, 6, 7) };
			playerEntity += new Statable() { PlayerState = PlayerStateType.None };

			playerEntity += new Drawable(new Animation(AnimationType.Idle,
													   Content.Load<Texture2D>("player0")),
										 new Animation(AnimationType.Running,
													   Content.Load<Texture2D>("player0"),
													   Content.Load<Texture2D>("player1"),
													   Content.Load<Texture2D>("player0"),
													   Content.Load<Texture2D>("player2")));



			Song song = Content.Load<Song>("01 The Guillotine Factory - Assembly Line");
			//Song song = Content.Load<Song>("13 H-Pizzle - Ghosts of a Fallen Empire");
			MediaPlayer.Play(song);
			MediaPlayer.Volume = 0.2f;

			base.Initialize();
		}

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.Escape)) 
				Exit();

			if (keyboard.IsKeyDown(Keys.Space) && previousKeyboard.IsKeyUp(Keys.Space))
			{
				mapSystem.GenerateMap();
				playerEntity.GetComponent<Positionable>().CurrentPosition = mapSystem.GetOpenSpaceNearEntrance();
			}
			
			/*
            if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released
                && entityPool.DoesEntityExist(hostileEntity))
                    hostileEntity.GetComponent<TransformComponent>().Position = new Vector2(mouse.Position.X - 16, mouse.Position.Y - 16);

            if (mouse.RightButton == ButtonState.Pressed && previousMouse.RightButton == ButtonState.Released
                && entityPool.DoesEntityExist(hostileEntity))
            {
                entityPool.DestroyEntity(ref hostileEntity);

                var fromTheCache = entityPool.CreateEntity("FromTheCache");

                IComponent[] components = new IComponent[]
                {
                    new TransformComponent() { Position = new Vector2(300, 256) },
                    new GraphicsComponent() { Texture = Content.Load<Texture2D>("player0") }
                };

                fromTheCache.AddComponents(components);
            }

            if (keyboard.IsKeyDown(Keys.R) && previousKeyboard.IsKeyUp(Keys.R) 
                    && entityPool.DoesEntityExist("Player"))
                        playerEntity.Switch();

			*/
            
			inputSystem?.Update(gameTime);
			collisionSystem?.Update(gameTime);
			physicsSystem?.Update(gameTime);
			mapSystem?.Update();

            previousMouse = mouse;
            previousKeyboard = keyboard;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
			Resolution.BeginDraw();

            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Resolution.getTransformationMatrix());
				drawingSystem?.Animate(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
