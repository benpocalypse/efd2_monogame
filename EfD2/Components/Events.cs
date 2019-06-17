using System;
using System.Collections;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Events : IComponent
	{
		Entity IComponent.entity { get; set; }

        public List<Event> EventList;
		public Events()
		{
            EventList = new List<Event>();
		}
	}

    public class Event : IEnumerable<Event>, IComponent
    {
        Entity IComponent.entity { get; set; }
        public GameEventType Type = GameEventType.None;
        public EventTrigger Trigger = EventTrigger.None;
        public bool Triggered = false;

        public IEnumerator GetEnumerator()
        {
            yield return Type;
        }

        IEnumerator<Event> IEnumerable<Event>.GetEnumerator()
        {
            return (System.Collections.Generic.IEnumerator<EfD2.Components.Event>)GetEnumerator();
        }
    }
}
