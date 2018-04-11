using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Collectible : IComponent
	{
		Entity IComponent.entity { get; set; }

		public CollectibleType Type = CollectibleType.None;

		public Collectible()
		{
		}
	}
}
