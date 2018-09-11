using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Collidable : IComponent
	{
		public Entity Owner { get; set; }

		public RectangleF BoundingBox { get; set; }
		public Entity entity { get; set; }
		public List<Entity> CollidingEntities;

		public Collidable()
		{
			CollidingEntities = new List<Entity>();
			BoundingBox = new RectangleF(0, 0, 8, 8);
		}
	}
}
