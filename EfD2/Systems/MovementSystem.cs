﻿using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;

namespace EfD2
{
	public class MovementSystem
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		public MovementSystem()
		{
		}

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(Positionable), typeof(Movable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public void Update(GameTime gameTime)
		{
			var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var position = e.GetComponent<Positionable>();
				var move = e.GetComponent<Movable>();
				var input = e.GetComponent<HasInput>();
				var draw = e.GetComponent<Drawable>();

				bool moving = false;

				position.PreviousPosition = position.CurrentPosition;
				move.PreviousDirection = move.CurrentDirection;

				if (input != null)
				{
					//if (GamePad.GetState(PlayerIndex.One).GamePadDPad//.Buttons.Up == ButtonState.Pressed)
					if (input.CurrentInput.Contains(Input.Right))
					{
						move.CurrentDirection = Direction.Right;
						draw.FlipOnXAxis = false;
						moving = true;
					}

					if (input.CurrentInput.Contains(Input.Left))
					{
						move.CurrentDirection = Direction.Left;
						draw.FlipOnXAxis = true;
						moving = true;
					}

					if (input.CurrentInput.Contains(Input.Up))
					{
						move.CurrentDirection = Direction.Up;
						moving = true;
					}

					if (input.CurrentInput.Contains(Input.Down))
					{
						move.CurrentDirection = Direction.Down;
						moving = true;
					}
				}

				if (moving == true)
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
				if (anim != null)
				{
					if (move.CurrentDirection != Direction.None)
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
