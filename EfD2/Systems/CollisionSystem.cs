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
			// FIXME - Do we really need Drawable here? The original intent was to use it as the collision box,
			//         but I think there were problems with this, IIRC.
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

					if (rect1.Intersects(rect2))
					{
						if (!col1.CollidingEntities.Contains(o))
							col1.CollidingEntities.Add(o);


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


						//if (!col2.CollidingEntities.Contains(e))
						//	col2.CollidingEntities.Add(e);
					}
					else
					{
						col1.CollidingEntities.Remove(o);
						//col2.CollidingEntities.Remove(e);
					}
				}

			}
		}
	}
}
