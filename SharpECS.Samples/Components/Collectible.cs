using System;
using System.Collections.Generic;
using SharpECS;

namespace EfD2.Components
{
	public class Collectible : IComponent
	{
		public Entity Owner { get; set; }

		public CollectibleType Type = CollectibleType.None;

		public Collectible()
		{
		}
	}
}
