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

namespace EfD2.Samples
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

		InputSystem inputSystem;
		CollisionSystem collisionSystem;
		PhysicsSystem physicsSystem;
		DrawingSystem drawingSystem;

        Entity playerEntity;
		Entity wall1Entity;
		Entity wall2Entity;

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

			// Systems will refresh when new Entities have compatible components added to them.
			drawingSystem = new DrawingSystem(entityPool);
			inputSystem = new InputSystem(entityPool);
			collisionSystem = new CollisionSystem(entityPool);
			physicsSystem = new PhysicsSystem(entityPool);
			drawingSystem = new DrawingSystem(entityPool);

			playerEntity = entityPool.CreateEntity("Player");
			wall1Entity = entityPool.CreateEntity("Wall1");
			wall2Entity = entityPool.CreateEntity("Wall2");


			// One way of adding components.
			playerEntity += new Positionable() { CurrentPosition = new Vector2(10, 10) };
			//playerEntity += new Drawable() { Texture = Content.Load<Texture2D>("player0") };
			playerEntity += new Movable() { MoveSpeed = 75 };
			playerEntity += new Collidable() { Type = EntityType.Player };


			List<Texture2D> tl = new List<Texture2D>();
			tl.Add(Content.Load<Texture2D>("player0"));

			Animation anim1 = new Animation();
			anim1.Type = AnimationType.Idle;
			anim1.FrameList = tl;

			tl = new List<Texture2D>();
			tl.Add(Content.Load<Texture2D>("player0"));
			tl.Add(Content.Load<Texture2D>("player1"));
			tl.Add(Content.Load<Texture2D>("player0"));
			tl.Add(Content.Load<Texture2D>("player2"));

			Animation anim2 = new Animation();
			anim2.Type = AnimationType.Running;
			anim2.FrameList = tl;

			playerEntity += new Drawable();
			var pAnim = playerEntity.GetComponent<Drawable>();
			pAnim.AddAnimation(anim1);
			pAnim.AddAnimation(anim2);


			tl = new List<Texture2D>();
			tl.Add(Content.Load<Texture2D>("wall1_1"));

			anim1 = new Animation();
			anim1.Type = AnimationType.None;
			anim1.FrameList = tl;

			wall1Entity += new Positionable() { CurrentPosition = new Vector2(60, 60) };
			wall1Entity += new Drawable();
			wall1Entity += new Collidable() { Type = EntityType.Wall };

			var wAnim = wall1Entity.GetComponent<Drawable>();
			wAnim.AddAnimation(anim1);

			wall2Entity += new Positionable() { CurrentPosition = new Vector2(100, 60) };
			wall2Entity += new Drawable();
			wall2Entity += new Collidable() { Type = EntityType.Wall };

			wAnim = wall2Entity.GetComponent<Drawable>();
			wAnim.AddAnimation(anim1);

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

            previousMouse = mouse;
            previousKeyboard = keyboard;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
			Resolution.BeginDraw();

            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Resolution.getTransformationMatrix());
				drawingSystem?.Animate(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
