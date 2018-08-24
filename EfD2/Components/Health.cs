using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Health : IComponent
	{
		Entity IComponent.entity { get; set; }

		public int Value = 0;

		public Health()
		{
		}
	}
}
