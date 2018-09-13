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

				switch (act.Type)
				{
					case ActorType.Player:
						{
							if (gameState.State == GameStateType.EnterMap)
							{
								Entity exit = EntityMatcher.GetEntity("OpenSpaceNextToEntrance");
								e.GetComponent<Positionable>().CurrentPosition = exit.GetComponent<Positionable>().CurrentPosition;
								e.GetComponent<Positionable>().PreviousPosition = exit.GetComponent<Positionable>().CurrentPosition;
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
