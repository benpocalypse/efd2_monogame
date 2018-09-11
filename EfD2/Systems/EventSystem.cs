using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;

namespace EfD2.Systems
{
	public class EventSystem
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;
		private MapSystem Map;

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(Event)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public EventSystem(ref MapSystem _map)
		{
			Map = _map;
		}

		public void Update(GameTime gameTime)
		{
			var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				if (e.GetComponent<Event>().Triggered == true)
				{
					var evt = e.GetComponent<Event>().Type;

					switch (evt)
					{
						case EventType.Exit:
							//Map.GenerateMap();
							e.GetComponent<Event>().Triggered = false;
							break;

						default:
							break;
					}
				}
			}
		}
	}
}
