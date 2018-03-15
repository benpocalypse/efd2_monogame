using System;
using SharpECS;

namespace EfD2.Components
{
	public class Movable : IComponent
	{
		public Entity Owner { get; set; }

		public bool CanMove { get; set; }

		public float MoveSpeed { get; set; } = 700;

		public Movable()
		{
			CanMove = true;
		}
	}
}
