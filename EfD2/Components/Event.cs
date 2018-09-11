using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Event : IComponent
	{
		Entity IComponent.entity { get; set; }

		public EventType Type = EventType.None;
		public EventTrigger Trigger = EventTrigger.None;
		public bool Triggered = false;

		public Event()
		{
		}
	}
}
