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
			get { return new Filter().AllOf(typeof(Positionable), typeof(Movable), typeof(Collidable)); }
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
			List<Entity> entitiesToRemove = new List<Entity>();

			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				foreach (IComponent ic in e.components.Where(_ => _.GetType() == typeof(Collidable) && ((Collidable)_).Colliding == true))
				{
					var col = ((Collidable)ic);

					switch (col.Type)
					{
						case EntityType.Player:
							HandlePlayerCollisions(ref col, e);
							break;

						case EntityType.Wall:
							break;
					}
				}
			}

			foreach (Entity e in entitiesToRemove)
			{
				EntityMatcher.Remove(e);
			}
		}

		private void HandlePlayerCollisions(ref Collidable collidable, Entity player)
		{
			foreach (Entity oe in collidable.CollidingEntities)
			{
				foreach (IComponent oic in oe.components.Where(_ => _.GetType() == typeof(Collidable)))
				{
					switch (((Collidable)oic).Type)
					{
						case EntityType.Wall:
							{
								var pos1 = player.GetComponent<Positionable>();
								var mov1 = player.GetComponent<Movable>();
								var col1 = player.GetComponent<Collidable>();

								pos1.CurrentPosition = pos1.PreviousPosition;
								mov1.CurrentDirection = Direction.None;
								mov1.PreviousDirection = Direction.None;
								mov1.Acceleration = 0;

								col1.Colliding = false;
								collidable.Colliding = false;
							}
							break;

						case EntityType.Exit:
							{
								// FIXME - Gah...how to do this without relying on the Name?
								//var player = EntityMatcher.GetEntity("Player");
								player.GetComponent<HasActorState>().ActorState = ActorStateType.HitExit;
							}
							break;

						case EntityType.Weapon:
							{
								
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
		}
	}
}
