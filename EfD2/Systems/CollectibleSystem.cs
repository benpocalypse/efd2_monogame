using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;

namespace EfD2.Systems
{
	public class CollectibleSystem 
		: IReactiveSystem

	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		public Filter filterActorMatch
		{
			get { return new Filter().AllOf(typeof(Collectible), typeof(Collidable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public CollectibleSystem()
		{
		}

        // FIXME - is this really the way we want to handle this? Couldn't this be done in the Collision or Physics systems?
		public void Update(GameTime gameTime)
		{
			List<Entity> entitiesToRemove = new List<Entity>();

			foreach (Entity collectibleEntity in EntityMatcher.GetMatchedEntities(filterActorMatch))
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
