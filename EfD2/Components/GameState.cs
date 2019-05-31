using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class GameState: IComponent
	{
		Entity IComponent.entity { get; set; }

		public GameStateType CurrentState { get; set; } = GameStateType.Unknown;
        public GameStateType PreviousState { get; set; } = GameStateType.Unknown;
        public GameStateType RequestedState { get; set; } = GameStateType.Intro;

        public GameState()
		{
		}
	}
}
