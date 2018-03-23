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
						move.CurrentDirection = MoveDirection.Right;
						draw.FlipOnXAxis = false;
						Moving = true;
					}

                    if (Keyboard.GetState().IsKeyDown(Keys.A)) 
					{ 
						move.CurrentDirection = MoveDirection.Left;
						draw.FlipOnXAxis = true;
						Moving = true;
					}

                    if (Keyboard.GetState().IsKeyDown(Keys.W)) 
					{ 
						move.CurrentDirection = MoveDirection.Up;
						Moving = true;
					}

                    if (Keyboard.GetState().IsKeyDown(Keys.S)) 
					{ 
						move.CurrentDirection = MoveDirection.Down;
						Moving = true;
					}

					if (Moving == true)
					{
						// Update our acceleration based on direction
						if (move.CurrentDirection == move.PreviousDirection)
						{
							if (move.Acceleration < move.MaxAcceleration)
								move.Acceleration += 0.1f;
						}
						else
						{
							move.Acceleration = 0.0f;
						}

						switch (move.CurrentDirection)
						{
							case MoveDirection.Right:
								position.SetX(position.CurrentPosition.X + ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case MoveDirection.Left:
								position.SetX(position.CurrentPosition.X - ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case MoveDirection.Up:
								position.SetY(position.CurrentPosition.Y - ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case MoveDirection.Down:
								position.SetY(position.CurrentPosition.Y + ((move.MoveSpeed * move.Acceleration) * delta));
								break;
						}
					}
					else
					{
						if (move.Acceleration > 0.0f)
							move.Acceleration -= 0.2f;

						if (move.Acceleration <= 0.0f)
						{
							move.Acceleration = 0.0f;
							move.CurrentDirection = MoveDirection.None;
						}

						switch (move.PreviousDirection)
						{
							case MoveDirection.Right:
								position.SetX(position.CurrentPosition.X + ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case MoveDirection.Left:
								position.SetX(position.CurrentPosition.X - ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case MoveDirection.Up:
								position.SetY(position.CurrentPosition.Y - ((move.MoveSpeed * move.Acceleration) * delta));
								break;

							case MoveDirection.Down:
								position.SetY(position.CurrentPosition.Y + ((move.MoveSpeed * move.Acceleration) * delta));
								break;
						}
					}

					var anim = Compatible[i].GetComponent<Drawable>();
					if(anim != null)
					{
						if(move.CurrentDirection != MoveDirection.None)
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
