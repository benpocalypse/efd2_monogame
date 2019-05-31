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
	{
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

        public Filter filterCollectibleMatch
        {
            get { return new Filter().AllOf(typeof(Collectible), typeof(Collidable)); }
        }

		public void Update(GameTime gameTime)
		{
            MarkCollisions();
            ReactToCollisions();
		}

        public void MarkCollisions()
        {
            foreach (Entity o in EntityMatcher.GetMatchedEntities(filterMovableCollidables))
            {
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
                        var ev1 = e.GetComponent<Events>();
                        if (ev1 != null && ev1.Trigger == EventTrigger.Collision)
                        {
                            ev1.Triggered = true;
                        }

                        var ev2 = o.GetComponent<Events>();
                        if (ev2 != null && ev2.Trigger == EventTrigger.Collision)
                        {
                            ev2.Triggered = true;
                        }

                        System.Console.WriteLine(o.Id + " is colliding with " + e.Id);
                    }
                    else
                    {
                        if (col2.CollidingEntities.Contains(e))
                            col2.CollidingEntities.Remove(e);
                    }
                }

            }
        }

        public void ReactToCollisions()
        {
            // FIXME - do stuff in here
            ReactToCollectibles();
        }

        public void ReactToCollectibles()
        {
            List<Entity> entitiesToRemove = new List<Entity>();

            foreach (Entity collectibleEntity in EntityMatcher.GetMatchedEntities(filterCollectibleMatch))
            {
                var collidable = collectibleEntity.GetComponent<Collidable>();
                var collectible = collectibleEntity.GetComponent<Collectible>();

                foreach (Entity collidingEntity in collidable.CollidingEntities)
                {
                    // If the entity the Collectible is colliding with has an Inventory...
                    if (collidingEntity.GetComponent<Inventory>() != null)
                    {
                        switch (collectible.Type)
                        {
                            case CollectibleType.Gold:
                                collidingEntity.GetComponent<Inventory>().Gold += collectible.Value;
                                break;
                        }

                        collidingEntity.GetComponent<Collidable>().CollidingEntities.Remove(collectibleEntity);

                        if (!entitiesToRemove.Contains(collectibleEntity))
                            entitiesToRemove.Add(collectibleEntity);
                    }
                }
            }

            foreach (Entity e in entitiesToRemove)
            {
                EntityMatcher.Remove(e);
            }
        }
    }
}
