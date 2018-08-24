using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class HasGameState: IComponent
	{
		Entity IComponent.entity { get; set; }

		public GameStateType GameState { get; set; } = GameStateType.Intro;

		public HasGameState()
		{
		}
	}
}
