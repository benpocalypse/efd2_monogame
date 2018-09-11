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
			get { return new Filter().AllOf(typeof(Positionable), typeof(Collidable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public PhysicsSystem()
		{
		}

		// FIXME - update and optimize this with more/better LINQ
		public void Update(GameTime gameTime)
		{
			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch).Where(_ => _.GetComponent<Movable>() != null))
			{
				if (e.GetComponent<Collidable>().CollidingEntities.Count > 0)
				{
					var entityPositionable = e.GetComponent<Positionable>();
					var entityMovable = e.GetComponent<Movable>();
					var entityCollidable = e.GetComponent<Collidable>();

					entityPositionable.CurrentPosition = entityPositionable.PreviousPosition;
					entityMovable.CurrentDirection = Direction.None;
					entityMovable.PreviousDirection = Direction.None;
					entityMovable.Acceleration = 0f;
				}
			}
		}

         /*
		private void HandlePlayerCollisions(ref Collidable collidable, Entity player)
		{
			foreach (Entity oe in collidable.CollidingEntities)
			{
				foreach (IComponent oic in oe.components.Where(_ => _.GetType() == typeof(Collidable)))
				{
					switch (((Collidable)oic).Physics)
					{
						case EntityType.Wall:
							{
								var entityPositionable = player.GetComponent<Positionable>();
								var entityMovable = player.GetComponent<Movable>();
								var entityCollidable = player.GetComponent<Collidable>();

								entityPositionable.CurrentPosition = entityPositionable.PreviousPosition;
								entityMovable.CurrentDirection = Direction.None;
								entityMovable.PreviousDirection = Direction.None;
								entityMovable.Acceleration = 0f;

								entityCollidable.Colliding = false;
								collidable.Colliding = false;
							}
							break;

						case EntityType.Exit:
							{
								player.GetComponent<HasActorState>().ActorStateList.Add(ActorStateType.HitExit);
							}
							break;

						case EntityType.Weapon:
							{
								player.GetComponent<HasActorState>().ActorStateList.Add(ActorStateType.Hurt);
							}
							break;
							
						case EntityType.Item:
							{
								//entitiesToRemove.Add(oe);
								collidable.Colliding = false;

								// FIXME - this is a hack. We need to make this generic by using the Collectible Component.
								if (!EntityMatcher.DoesEntityExist("message!"))
								{
									var goldPos = EntityMatcher.GetEntity("gold").GetComponent<Positionable>();
									Entity collectionText = new Entity("message!");

									collectionText.AddComponent(new Positionable { CurrentPosition = (goldPos.CurrentPosition / 8), ZOrder = (float)DisplayLayer.Text });
									collectionText.AddComponent(new Ephemeral { PersistTime = 2.0, TotalTime = 2.0f });

									HasText t = new HasText();
									t.Text.Add("100!");
									t.Homgeneous = false;
									t.Border = true;
									collectionText.AddComponent(t);
								}
							}
							break;
					}
				}
			}
			//break;
			*/
	}
}
