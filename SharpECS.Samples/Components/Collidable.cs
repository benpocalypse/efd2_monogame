using System;
using System.Collections.Generic;
using SharpECS;

namespace EfD2.Components
{
	public class Collidable : IComponent
	{
		public Entity Owner { get; set; }

		public bool Colliding { get; set; }

		public EntityType Type;

		public List<Entity> CollidingEntities;

		public Collidable()
		{
			CollidingEntities = new List<Entity>();
			Type = EntityType.None;
		}
	}
}
