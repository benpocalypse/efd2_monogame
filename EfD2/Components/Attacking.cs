using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Attacking : IComponent
	{
		Entity IComponent.entity { get; set; }

		public AttackStateType AttactState = AttackStateType.NotAttacking;

		public Attacking()
		{
		}
	}
}
