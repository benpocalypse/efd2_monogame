using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using EfD2.Components;

namespace EfD2.Systems
{
	internal class PhysicsSystem
		: EntitySystem
	{
		public PhysicsSystem(EntityPool pool)
			: base(pool, typeof(Positionable), typeof(Collidable))
		{ }

		public void Update(GameTime gameTime)
		{
			foreach (Entity e in Compatible)
			{
				foreach (IComponent ic in e.Components.Where(_ => _.GetType() == typeof(Collidable) && ((Collidable)_).Colliding == true))
				{
					var col = ((Collidable)ic);

					switch (col.Type)
					{
						case EntityType.Player:
							foreach (Entity oe in col.CollidingEntities)
							{
								foreach (IComponent oic in oe.Components.Where(_ => _.GetType() == typeof(Collidable)))
								{
									if (((Collidable)oic).Type == EntityType.Wall)
									{
										var pos1 = e.GetComponent<Positionable>();
										var mov1 = e.GetComponent<Movable>();
										var col1 = e.GetComponent<Collidable>();

										pos1.CurrentPosition = pos1.PreviousPosition;
										mov1.CurrentDirection = MoveDirection.None;
										mov1.PreviousDirection = MoveDirection.None;
										mov1.Acceleration = 0;
										col1.Colliding = false;
									}
								}
							}
							break;

						case EntityType.Wall:
							break;
					}
				}
			}
		}
	}
}
