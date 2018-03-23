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

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
			Resolution.Init(ref graphics);
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

			playerEntity = entityPool.CreateEntity("Player");

			// One way of adding components.
			playerEntity += new Positionable() { CurrentPosition = new Vector2(50, 50), ZOrder = 1.0f };
			playerEntity += new Movable() { MoveSpeed = 75 };
			playerEntity += new Collidable() { Type = EntityType.Player };

			playerEntity += new Drawable(new Animation(AnimationType.Idle, 
			                                           Content.Load<Texture2D>("player0")),
										 new Animation(AnimationType.Running, 
			                                           Content.Load<Texture2D>("player0"),
			                                           Content.Load<Texture2D>("player1"),
			                                           Content.Load<Texture2D>("player0"),
			                                           Content.Load<Texture2D>("player2")));

			CreateRoom();

			base.Initialize();
		}

		private void CreateRoom()
		{
			var wall1Anim = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_1"));
			var wall2Anim = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_2"));
			var wall3Anim = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_3"));
			var wall4Anim = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_4"));
			var wall5Anim = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_5"));
			var wall6Anim = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_6"));
			var floorAnim = new Animation(AnimationType.None, Content.Load<Texture2D>("floor1_2"));

			for(int i = 0; i < 10; i++)
			{
				Entity wall0 = entityPool.CreateEntity("Wall0" + i);
				Entity wall1 = entityPool.CreateEntity("Wall1" + i);
				Entity wall2 = entityPool.CreateEntity("Wall2" + i);
				Entity wall3 = entityPool.CreateEntity("Wall3" + i);
				Entity wall4 = entityPool.CreateEntity("Wall4" + i);
				Entity wall5 = entityPool.CreateEntity("Wall5" + i);
				Entity wall6 = entityPool.CreateEntity("Wall6" + i);
				Entity wall7 = entityPool.CreateEntity("Wall7" + i);
				Entity wall8 = entityPool.CreateEntity("Wall8" + i);
				Entity wall9 = entityPool.CreateEntity("Wall9" + i);

				Entity floor0 = entityPool.CreateEntity("Floor0" + i);
				Entity floor1 = entityPool.CreateEntity("Floor1" + i);
				Entity floor2 = entityPool.CreateEntity("Floor2" + i);
				Entity floor3 = entityPool.CreateEntity("Floor3" + i);
				Entity floor4 = entityPool.CreateEntity("Floor4" + i);
				Entity floor5 = entityPool.CreateEntity("Floor5" + i);
				Entity floor6 = entityPool.CreateEntity("Floor6" + i);
				Entity floor7 = entityPool.CreateEntity("Floor7" + i);


				switch(i)
				{
					case 0:
						wall0 += new Positionable() { CurrentPosition = new Vector2(20 + 0, 20 + (i * 8)) };
						wall0 += new Drawable(wall5Anim);
						wall0 += new Collidable() { Type = EntityType.Wall };

						wall1 += new Positionable() { CurrentPosition = new Vector2(20 + 8, 20 + (i * 8)) };
						wall1 += new Drawable(wall1Anim);
						wall1 += new Collidable() { Type = EntityType.Wall };

						wall2 += new Positionable() { CurrentPosition = new Vector2(20 + 16, 20 + (i * 8)) };
						wall2 += new Drawable(wall1Anim);
						wall2 += new Collidable() { Type = EntityType.Wall };

						wall3 += new Positionable() { CurrentPosition = new Vector2(20 + 24, 20 + (i * 8)) };
						wall3 += new Drawable(wall1Anim);
						wall3 += new Collidable() { Type = EntityType.Wall };

						wall4 += new Positionable() { CurrentPosition = new Vector2(20 + 32, 20 + (i * 8)) };
						wall4 += new Drawable(wall1Anim);
						wall4 += new Collidable() { Type = EntityType.Wall };

						wall5 += new Positionable() { CurrentPosition = new Vector2(20 + 40, 20 + (i * 8)) };
						wall5 += new Drawable(wall1Anim);
						wall5 += new Collidable() { Type = EntityType.Wall };

						wall6 += new Positionable() { CurrentPosition = new Vector2(20 + 48, 20 + (i * 8)) };
						wall6 += new Drawable(wall1Anim);
						wall6 += new Collidable() { Type = EntityType.Wall };

						wall7 += new Positionable() { CurrentPosition = new Vector2(20 + 56, 20 + (i * 8)) };
						wall7 += new Drawable(wall1Anim);
						wall7 += new Collidable() { Type = EntityType.Wall };

						wall8 += new Positionable() { CurrentPosition = new Vector2(20 + 64, 20 + (i * 8)) };
						wall8 += new Drawable(wall1Anim);
						wall8 += new Collidable() { Type = EntityType.Wall };

						wall9 += new Positionable() { CurrentPosition = new Vector2(20 + 72, 20 + (i * 8)) };
						wall9 += new Drawable(wall6Anim);
						wall9 += new Collidable() { Type = EntityType.Wall };
						break;
						
					case 9:
						wall0 += new Positionable() { CurrentPosition = new Vector2(20+0, 20+(i*8)) };
						wall0 += new Drawable(wall4Anim);
						wall0 += new Collidable() { Type = EntityType.Wall };

						wall1 += new Positionable() { CurrentPosition = new Vector2(20+8, 20+(i*8)) };
						wall1 += new Drawable(wall1Anim);
						wall1 += new Collidable() { Type = EntityType.Wall };

						wall2 += new Positionable() { CurrentPosition = new Vector2(20+16, 20+(i*8)) };
						wall2 += new Drawable(wall1Anim);
						wall2 += new Collidable() { Type = EntityType.Wall };

						wall3 += new Positionable() { CurrentPosition = new Vector2(20+24, 20+(i*8)) };
						wall3 += new Drawable(wall1Anim);
						wall3 += new Collidable() { Type = EntityType.Wall };

						wall4 += new Positionable() { CurrentPosition = new Vector2(20+32, 20+(i*8)) };
						wall4 += new Drawable(wall1Anim);
						wall4 += new Collidable() { Type = EntityType.Wall };

						wall5 += new Positionable() { CurrentPosition = new Vector2(20+40, 20+(i*8)) };
						wall5 += new Drawable(wall1Anim);
						wall5 += new Collidable() { Type = EntityType.Wall };

						wall6 += new Positionable() { CurrentPosition = new Vector2(20+48, 20+(i*8)) };
						wall6 += new Drawable(wall1Anim);
						wall6 += new Collidable() { Type = EntityType.Wall };

						wall7 += new Positionable() { CurrentPosition = new Vector2(20+56, 20+(i*8)) };
						wall7 += new Drawable(wall1Anim);
						wall7 += new Collidable() { Type = EntityType.Wall };

						wall8 += new Positionable() { CurrentPosition = new Vector2(20+64, 20+(i*8)) };
						wall8 += new Drawable(wall1Anim);
						wall8 += new Collidable() { Type = EntityType.Wall };

						wall9 += new Positionable() { CurrentPosition = new Vector2(20+72, 20+(i*8)) };
						wall9 += new Drawable(wall3Anim);
						wall9 += new Collidable() { Type = EntityType.Wall };
						break;

					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
						wall0 += new Positionable() { CurrentPosition = new Vector2(20+0, 20+(i*8)) };
						wall0 += new Drawable(wall2Anim);
						wall0 += new Collidable() { Type = EntityType.Wall };

						floor0 += new Positionable() { CurrentPosition = new Vector2(20+8, 20+(i*8)) };
						floor0 += new Drawable(floorAnim);

						floor1 += new Positionable() { CurrentPosition = new Vector2(20+16, 20+(i*8)) };
						floor1 += new Drawable(floorAnim);

						floor2 += new Positionable() { CurrentPosition = new Vector2(20+24, 20+(i*8)) };
						floor2 += new Drawable(floorAnim);
					
						floor3 += new Positionable() { CurrentPosition = new Vector2(20+32, 20+(i*8)) };
						floor3 += new Drawable(floorAnim);

						floor4 += new Positionable() { CurrentPosition = new Vector2(20+40, 20+(i*8)) };
						floor4 += new Drawable(floorAnim);

						floor5 += new Positionable() { CurrentPosition = new Vector2(20+48, 20+(i*8)) };
						floor5 += new Drawable(floorAnim);

						floor6 += new Positionable() { CurrentPosition = new Vector2(20+56, 20+(i*8)) };
						floor6 += new Drawable(floorAnim);

						floor7 += new Positionable() { CurrentPosition = new Vector2(20+64, 20+(i*8)) };
						floor7 += new Drawable(floorAnim);

						wall1 += new Positionable() { CurrentPosition = new Vector2(20+72, 20+(i*8)) };
						wall1 += new Drawable(wall2Anim);
						wall1 += new Collidable() { Type = EntityType.Wall };
						break;
				}
			}
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

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Resolution.getTransformationMatrix());
				drawingSystem?.Animate(spriteBatch, gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
