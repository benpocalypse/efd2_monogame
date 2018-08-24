using System;
using ECS;

namespace EfD2.Components
{
	public class HasActorState : IComponent
	{
		Entity IComponent.entity { get; set; }

		public ActorStateType ActorState { get; set; } = ActorStateType.None;

		public HasActorState()
		{
		}
	}
}
