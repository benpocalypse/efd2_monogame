using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Inventory : IComponent
	{
		Entity IComponent.entity { get; set; }

		public int Gold = 0;
		public List<CollectibleType> Items = new List<CollectibleType>();

		public Inventory()
		{
		}
	}
}
