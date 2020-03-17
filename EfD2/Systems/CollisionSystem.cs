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
    public sealed class CollisionSystem
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

        private Filter filterCollectibleMatch
        {
            get { return new Filter().AllOf(typeof(Collectible), typeof(Collidable)); }
        }

        private Filter filterEventMatch
        {
            get { return new Filter().AllOf(typeof(Event), typeof(Collidable)); }
        }

        public void Update(GameTime gameTime)
        {
            MarkCollisions();
            ReactToCollisions();
        }

        private void MarkCollisions()
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
                        // FIXME - This is running CONSTANTLY
                        if (col2.CollidingEntities.Contains(e))
                            col2.CollidingEntities.Remove(e);
                    }
                }

            }
        }

        private void ReactToCollisions()
        {
            // FIXME - do stuff in here
            ReactToCollectibles();
            GenerateEvents();
        }

        private void ReactToCollectibles()
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

                            case CollectibleType.Health:
                                if (collidingEntity.GetComponent<Health>().Value < collidingEntity.GetComponent<Health>().Max)
                                    collidingEntity.GetComponent<Health>().Value += 1;
                                break;
                        }

                        collidingEntity.GetComponent<Collidable>().CollidingEntities.Remove(collectibleEntity);

                        if (!entitiesToRemove.Contains(collectibleEntity))
                            entitiesToRemove.Add(collectibleEntity);
                    }
                }
            }

            EntityMatcher.Remove(entitiesToRemove);
        }

        private void GenerateEvents()
        {
            Globals g = Globals.Instance;
            foreach (Entity e in EntityMatcher.GetMatchedEntities(filterEventMatch).Where(_ => _.GetComponent<Event>().Triggered == true))
            {
                // If the player hits the exit, trigger the game event.
                if (e.Id.Equals(g.LevelExitTitle) && e.GetComponent<Collidable>().CollidingEntities.Contains(EntityMatcher.GetEntity(g.PlayerTitle)))
                {
                    EntityMatcher.GetEntity(g.GameTitle)
                            .GetComponent<Events>()
                            .EventList
                            .Add(new Event()
                            { Triggered = true, Trigger = EventTrigger.Collision, Type = GameEventType.ExitedLevel });
                }
            }
        }
    }
}
