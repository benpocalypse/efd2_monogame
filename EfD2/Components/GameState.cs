using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class GameState: IComponent
	{
		Entity IComponent.entity { get; set; }

		public GameStateType State { get; set; } = GameStateType.Intro;

		public GameState()
		{
		}
	}
}
