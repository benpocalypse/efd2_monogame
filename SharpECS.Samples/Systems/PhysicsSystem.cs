using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using EfD2.Components;

namespace EfD2.Samples
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
				foreach (IComponent ic in e.Components.Where(_ => _.GetType() == typeof(Collidable)))
				{
					var col = ((Collidable)ic);

					if (col.Colliding == true)
					{
						switch (col.Type)
						{
							case EntityType.Player:
								foreach (Entity oe in col.CollidingEntities)
								{
									foreach (IComponent oic in oe.Components.Where(_ => _.GetType() == typeof(Collidable)))
									{
										if (((Collidable)oic).Type == EntityType.Wall)
										{
											((Positionable)ic).CurrentPosition = ((Positionable)ic).PreviousPosition;
											((Movable)ic).Direction = MoveDirection.None;
											((Movable)ic).MoveSpeed = 0f;
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
}
