using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;

namespace EfD2.Systems
{
	public class ActorSystem
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(Actor)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public ActorSystem()
		{
		}

		public void Update(GameTime gameTime)
		{
			var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
			var gameEntity = EntityMatcher.GetEntity("The Game");
			var gameState = gameEntity.GetComponent<GameState>();

			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var act = e.GetComponent<Actor>();

                // FIXME - Is this really where/how we should handle this?
				switch (act.Type)
				{
					case ActorType.Player:
						{
							if (gameState.State == GameStateType.EnterMap)
							{
                                Entity openSpaceNearExit = EntityMatcher.GetEntity("OpenSpaceNextToEntrance");

                                // FIXME - Need to center the player here, not upper-left align.
                                var col = e.GetComponent<Collidable>();
								e.GetComponent<Positionable>().CurrentPosition = 
                                    new Vector2(
                                                openSpaceNearExit.GetComponent<Positionable>().CurrentPosition.X + ((8-col.BoundingBox.Width)/2),
                                                openSpaceNearExit.GetComponent<Positionable>().CurrentPosition.Y + ((8 - col.BoundingBox.Height ) / 2)
                                                );
                                e.GetComponent<Positionable>().PreviousPosition = e.GetComponent<Positionable>().CurrentPosition;

                                // FIXME - this isn't the right place to handle this. When changing maps, all entities CollidingEntities should be cleared.
                                col.CollidingEntities.Clear();
                             }
						}
						break;

					default:
						break;
				}
			}
		}


		private void MovePlayerNextToEntrance(ref Positionable playerPosition)
		{
			
		}
	}
}
