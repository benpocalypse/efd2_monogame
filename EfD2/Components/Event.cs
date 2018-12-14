using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Event : IComponent
	{
		Entity IComponent.entity { get; set; }

		public GameEventType Type = GameEventType.None;
		public EventTrigger Trigger = EventTrigger.None;
		public bool Triggered = false;

		public Event()
		{
		}
	}
}
