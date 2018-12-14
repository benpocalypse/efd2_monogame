using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;
using EfD2.Helpers;

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

        private Filter filterMovableCollidables
        {
            get { return new Filter().AllOf(typeof(Positionable), typeof(Collidable), typeof(Movable)); }
        }

        public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public void Update(GameTime gameTime)
		{
            foreach (Entity o in EntityMatcher.GetMatchedEntities(filterMovableCollidables))
			{
                //foreach (Entity o in EntityMatcher.GetMatchedEntities(filterStationaryCollidables).Where(_ => !_.Equals(e))) // && _.GetComponent<Movable>() != null))
                foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch).Where(_ => !_.Equals(o)))
                {
                    var pos1 = e.GetComponent<Positionable>();
                    var col1 = e.GetComponent<Collidable>();
                    var rect1 = new RectangleF(pos1.CurrentPosition.X, pos1.CurrentPosition.Y, col1.BoundingBox.Width, col1.BoundingBox.Height);

                    var pos2 = o.GetComponent<Positionable>();
					var col2 = o.GetComponent<Collidable>();
					var rect2 = new RectangleF(pos2.CurrentPosition.X, pos2.CurrentPosition.Y, col2.BoundingBox.Width, col2.BoundingBox.Height);

					// If there is a collision...
					if (rect1.Intersects(rect2))
					{
						// add the collision to the movable/colliding entity.
						if (!col2.CollidingEntities.Contains(e))
                            col2.CollidingEntities.Add(e);

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

                        System.Console.WriteLine(o.Id + " is colliding with " + e.Id);
                    }
					else
					{
						if(col2.CollidingEntities.Contains(e))
							col2.CollidingEntities.Remove(e);
					}
				}

			}
		}
	}
}
