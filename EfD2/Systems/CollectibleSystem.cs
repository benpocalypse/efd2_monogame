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

		public Filter filterMatch
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

		public void Update(GameTime gameTime)
		{
			List<Entity> entitiesToRemove = new List<Entity>();

			foreach (Entity collectibleEntity in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var collidable = collectibleEntity.GetComponent<Collidable>();
				var collectible = collectibleEntity.GetComponent<Collectible>();

				foreach (Entity collidingEntity in collidable.CollidingEntities)
				{
                    // If the entity the Collectible is colliding with has an Inventory...
					if (collidingEntity.GetComponent<Inventory>() != null)
					{
						if (collectible.Type == CollectibleType.Gold)
						{
							collidingEntity.GetComponent<Inventory>().Gold += collectible.Value;

							collidingEntity.GetComponent<Collidable>().CollidingEntities.Remove(collectibleEntity);

							if (!entitiesToRemove.Contains(collectibleEntity))
								entitiesToRemove.Add(collectibleEntity);
						}
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
