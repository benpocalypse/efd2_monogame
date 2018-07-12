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
			List<Entity> entitiesToRemove = new List<Entity>();

			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				foreach (IComponent ic in e.components.Where(_ => _.GetType() == typeof(Collidable) && ((Collidable)_).Colliding == true))
				{
					var col = ((Collidable)ic);

					switch (col.Type)
					{
						case EntityType.Player:
							foreach (Entity oe in col.CollidingEntities)
							{
								foreach (IComponent oic in oe.components.Where(_ => _.GetType() == typeof(Collidable)))
								{
									switch (((Collidable)oic).Type)
									{
										case EntityType.Wall:
											{
												var pos1 = e.GetComponent<Positionable>();
												var mov1 = e.GetComponent<Movable>();
												var col1 = e.GetComponent<Collidable>();

												pos1.CurrentPosition = pos1.PreviousPosition;
												mov1.CurrentDirection = Direction.None;
												mov1.PreviousDirection = Direction.None;
												mov1.Acceleration = 0;

												col1.Colliding = false;
												col.Colliding = false;
											}
											break;

										case EntityType.Exit:
											{
												var player = EntityMatcher.GetEntity("Player");
												player.GetComponent<PlayerStatable>().PlayerState = PlayerStateType.HitExit;
											}
											break;

										case EntityType.Item:
											{
												entitiesToRemove.Add(oe);
												col.Colliding = false;

												if (!EntityMatcher.DoesEntityExist("message!"))
												{
													var goldPos = EntityMatcher.GetEntity("gold").GetComponent<Positionable>();
													Entity collectionText = new Entity("message!");

													collectionText.AddComponent(new Positionable { CurrentPosition = (goldPos.CurrentPosition/8), ZOrder = 1.0f });
													collectionText.AddComponent(new Ephemeral { PersistTime = 5.0, TotalTime = 5.0f });
													HasText t = new HasText();
													t.Text.Add("+100!");
													t.Homgeneous = false;
													t.Border = false;
													collectionText.AddComponent(t);
												}
											}
											break;
									}
								}
							}
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
	}
}
