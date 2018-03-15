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

namespace SharpECS.Samples
{
    internal class Game 
        : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState keyboard;
        KeyboardState previousKeyboard;

        MouseState mouse;
        MouseState previousMouse;

        EntityPool entityPool;

        DrawingSystem drawingSystem;
		InputSystem inputSystem;

		Texture2D playerTexture;

        Entity playerEntity;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
			Resolution.Init(ref graphics);
            Content.RootDirectory = "Content";

			Resolution.SetVirtualResolution(256, 224);
			Resolution.SetResolution(1280, 800, false);

            IsMouseVisible = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }

		protected override void Initialize()
		{
			entityPool = EntityPool.New("EntityPool");

			playerEntity = entityPool.CreateEntity("Player");

			// Systems will refresh when new Entities have compatible components added to them.
			drawingSystem = new DrawingSystem(entityPool);
			inputSystem = new InputSystem(entityPool);

			// One way of adding components.
			playerEntity += new Positionable() { Position = new Vector2(10, 10) };
			playerEntity += new Drawable() { Texture = Content.Load<Texture2D>("player0") };
			playerEntity += new Movable() { MoveSpeed = 100 };
			//playerEntity += new ControllerComponent() { MoveSpeed = 100 };

			/*
			// Alternate way.
			hostileEntity.AddComponents
			(
				new GraphicsComponent() { Texture = Content.Load<Texture2D>("Sprite2") },
				new TransformComponent() { Position = new Vector2(350, 200) }
			);
			*/

			/*
			var newEntity = entityPool.CreateEntity("NewEntity");
			newEntity.AddComponents(new TransformComponent(), new GraphicsComponent());

			newEntity.GetComponent<TransformComponent>().Position = new Vector2(50, 50);
			newEntity.GetComponent<GraphicsComponent>().Texture = Content.Load<Texture2D>("player0");
			*/



			base.Initialize();
		}

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

			playerTexture = Content.Load<Texture2D>("player0");
        }

        protected override void UnloadContent()
        {
            Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.Escape)) Exit();

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

            previousMouse = mouse;
            previousKeyboard = keyboard;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
			Resolution.BeginDraw();

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Resolution.getTransformationMatrix());
            	drawingSystem?.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
