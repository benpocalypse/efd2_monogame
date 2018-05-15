using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class GameStatable: IComponent
	{
		Entity IComponent.entity { get; set; }

		public GameStateType PlayerState { get; set; } = GameStateType.Intro;

		public GameStatable()
		{
		}
	}
}
