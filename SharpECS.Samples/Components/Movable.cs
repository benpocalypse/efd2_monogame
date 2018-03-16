using System;
using SharpECS;

namespace EfD2.Components
{
	public class Movable : IComponent
	{
		public Entity Owner { get; set; }

		public float MoveSpeed { get; set; } = 700;
		public MoveDirection Direction { get; set; } = MoveDirection.None;

		public Movable()
		{
		}
	}
}
