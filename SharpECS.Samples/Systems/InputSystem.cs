using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using EfD2.Components;

namespace EfD2.Systems
{
	internal class InputSystem 
		: EntitySystem
	{
		public InputSystem(EntityPool pool)
			: base(pool, typeof(Positionable), typeof(Movable), typeof(Drawable))
        { }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < Compatible.Count; i++)
            {
                if (Compatible[i].State == EntityState.Active)
                {
                    var position = Compatible[i].GetComponent<Positionable>();
					var move = Compatible[i].GetComponent<Movable>();
					var draw = Compatible[i].GetComponent<Drawable>();
					bool Moving = false;

					position.PreviousPosition = position.CurrentPosition;
					move.PreviousDirection = move.CurrentDirection;

                    if (Keyboard.GetState().IsKeyDown(Keys.D)) 
					{
						move.CurrentDirection = Direction.Right;
						draw.FlipOnXAxis = false;
						Moving = true;
					}

                    if (Keyboard.GetState().IsKeyDown(Keys.A)) 
					{ 
						move.CurrentDirection = Direction.Left;
						draw.FlipOnXAxis = true;
						Moving = true;
					}

                    if (Keyboard.GetState().IsKeyDown(Keys.W)) 
					{ 
						move.CurrentDirection = Direction.Up;
						Moving = true;
					}

                    if (Keyboard.GetState().IsKeyDown(Keys.S)) 
					{ 
						move.CurrentDirection = Direction.Down;
						Moving = true;
					}

					if (Moving == true)
					{
						// Update our acceleration based on direction
						if (move.CurrentDirection == move.PreviousDirection)
						{
							if (move.Acceleration < move.MaxAcceleration)
								move.Acceleration += Globals.Acceleration;
						}
						else
						{
							move.Acceleration = 0.0f;
						}

						switch (move.CurrentDirection)
						{
							case Direction.Right:
								position.SetX(position.CurrentPosition.X + ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case Direction.Left:
								position.SetX(position.CurrentPosition.X - ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case Direction.Up:
								position.SetY(position.CurrentPosition.Y - ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case Direction.Down:
								position.SetY(position.CurrentPosition.Y + ((move.MoveSpeed * move.Acceleration) * delta));
								break;
						}
					}
					else
					{
						if (move.Acceleration > 0.0f)
							move.Acceleration -= Globals.Decceleration;

						if (move.Acceleration <= 0.0f)
						{
							move.Acceleration = 0.0f;
							move.CurrentDirection = Direction.None;
						}

						switch (move.PreviousDirection)
						{
							case Direction.Right:
								position.SetX(position.CurrentPosition.X + ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case Direction.Left:
								position.SetX(position.CurrentPosition.X - ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case Direction.Up:
								position.SetY(position.CurrentPosition.Y - ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case Direction.Down:
								position.SetY(position.CurrentPosition.Y + ((move.MoveSpeed * move.Acceleration) * delta));
								break;
						}
					}

					var anim = Compatible[i].GetComponent<Drawable>();
					if(anim != null)
					{
						if(move.CurrentDirection != Direction.None)
						{
							anim.Type = AnimationType.Running;
						}
						else
						{
							anim.Type = AnimationType.Idle;
						}
					}
                }
            }
        }
	}
}
