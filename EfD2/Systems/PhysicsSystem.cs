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
	internal class PhysicsSystem
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(Positionable), typeof(Collidable), typeof(Movable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public PhysicsSystem()
		{
		}

		// FIXME - Have this take direction, mass, etc into account in the future.
		public void Update(GameTime gameTime)
		{
			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch).Where(_ => _.GetComponent<Movable>() != null))
			{
				// If the entity is colliding, have it react to physics.
				if (e.GetComponent<Collidable>().CollidingEntities.Count > 0)
				{
					var entityPositionable = e.GetComponent<Positionable>();
					var entityMovable = e.GetComponent<Movable>();

					entityPositionable.CurrentPosition = entityPositionable.PreviousPosition;
					entityMovable.CurrentDirection = Direction.None;
					entityMovable.PreviousDirection = Direction.None;
					entityMovable.Acceleration = 0f;
				}
			}
		}
	}
}
