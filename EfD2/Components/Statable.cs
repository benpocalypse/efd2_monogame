using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Statable : IComponent
	{
		Entity IComponent.entity { get; set; }

		public PlayerStateType PlayerState { get; set;} = PlayerStateType.None;

		public Statable()
		{
		}
	}
}
