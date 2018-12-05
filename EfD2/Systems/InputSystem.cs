using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;

namespace EfD2.Systems
{
	public class InputSystem 
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(HasInput)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public InputSystem()
        { 
		}

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var input = e.GetComponent<HasInput>();

				// FIXME - Might not want to clear this every time
				input.CurrentInput.Clear();

				if (Keyboard.GetState().IsKeyDown(Keys.D))
				{
					input.CurrentInput.Add(Input.Right);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.A))
				{
					input.CurrentInput.Add(Input.Left);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.W))
				{
					input.CurrentInput.Add(Input.Up);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.S))
				{
					input.CurrentInput.Add(Input.Down);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.J))
				{
					input.CurrentInput.Add(Input.A);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.K))
				{
					input.CurrentInput.Add(Input.B);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.Space))
				{
					input.CurrentInput.Add(Input.Start);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.Enter))
				{
					input.CurrentInput.Add(Input.Select);
				}
			}
			/*

            foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
            {
                var position = e.GetComponent<Positionable>();
				var move = e.GetComponent<Movable>();
				var draw = e.GetComponent<Drawable>();
				bool Moving = false;

				position.PreviousPosition = position.CurrentPosition;
				move.PreviousDirection = move.CurrentDirection;

				//if (GamePad.GetState(PlayerIndex.One).GamePadDPad//.Buttons.Up == ButtonState.Pressed)
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

				var anim = e.GetComponent<Drawable>();
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
            */
        }
	}
}
