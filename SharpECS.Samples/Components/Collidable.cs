using System;
using SharpECS;

namespace EfD2.Components
{
	public class Collidable : IComponent
	{
		public Entity Owner { get; set; }

		public Collidable()
		{
		}
	}
}
