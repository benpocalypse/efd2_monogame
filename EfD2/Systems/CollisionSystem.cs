using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;

namespace EfD2.Systems
{
	public class CollisionSystem
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		public CollisionSystem()
		{ 
		}

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(Positionable), typeof(Collidable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public void Update(GameTime gameTime)
		{
			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var pos1 = e.GetComponent<Positionable>();
				var col1 = e.GetComponent<Collidable>();
				var rect1 = new RectangleF(pos1.CurrentPosition.X, pos1.CurrentPosition.Y, col1.BoundingBox.Width, col1.BoundingBox.Height);

				foreach (Entity o in EntityMatcher.GetMatchedEntities(filterMatch).Where(_ => !_.Equals(e))) // && _.GetComponent<Movable>() != null))
				{
					var pos2 = o.GetComponent<Positionable>();
					var col2 = o.GetComponent<Collidable>();
					var rect2 = new RectangleF(pos2.CurrentPosition.X, pos2.CurrentPosition.Y, col2.BoundingBox.Width, col2.BoundingBox.Height);

					// If there is a collision...
					if (rect1.Intersects(rect2))
					{
						// add the collision to the colliding entity.
						if (!col1.CollidingEntities.Contains(o))
							col1.CollidingEntities.Add(o);

						// Now, if there are events associated with the collision, flag them.
						var ev1 = e.GetComponent<Event>();
						if (ev1 != null && ev1.Trigger == EventTrigger.Collision)
						{
							ev1.Triggered = true;
						}

						var ev2 = o.GetComponent<Event>();
						if (ev2 != null && ev2.Trigger == EventTrigger.Collision)
						{
							ev2.Triggered = true;
						}

                        System.Console.WriteLine(e.Id + " is colliding with " + o.Id);
                    }
					else
					{
						if(col1.CollidingEntities.Contains(o))
							col1.CollidingEntities.Remove(o);
					}
				}

			}
		}
	}
}
