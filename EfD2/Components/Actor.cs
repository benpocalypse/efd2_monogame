using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Actor : IComponent
	{
		Entity IComponent.entity { get; set; }

		public ActorType Type { get; set; } = ActorType.None;

		public Actor()
		{
		}
	}
}
