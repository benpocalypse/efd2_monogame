using System;
using System.Collections.Generic;
using ECS;
using EfD2.Helpers;

namespace EfD2.Components
{
	public class Collidable : IComponent
	{
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
