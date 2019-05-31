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
    /// <summary>
    /// This system handles the State Machine for the game only. Nothing more.
    /// </summary>
	internal class GameStateSystem 
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(Event)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public GameStateSystem()
		{
			
		}

		public void Update(GameTime gameTime)
		{
			bool stateChanged = false;

			var gameState = EntityMatcher.GetEntity("The Game").GetComponent<GameState>();

			// First, process any Event related changes to the GameState
			switch (gameState.State)
			{
				case GameStateType.Intro:
					gameState.State = GameStateType.TitleScreen;
					stateChanged = true;
					break;

				case GameStateType.TitleScreen:
					gameState.State = GameStateType.EnterMap;
					stateChanged = true;
					break;
					
				case GameStateType.EnterMap:
					gameState.State = GameStateType.Playing;
					stateChanged = true;
					break;
					
				case GameStateType.Playing:
					break;
					
			}

			// If we didn't change states as a result of of our normal State Machine, 
			// then process Events that could change the GameState.
			if (stateChanged == false)
			{
				foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
				{
					var ev = e.GetComponent<Event>();

					if (ev.Triggered == true)
					{
						switch (ev.Type)
						{
							case GameEventType.PlayerHitExit:
								gameState.State = GameStateType.EnterMap;
								break;
						}
					}
				}
			}
		}
	}
}
