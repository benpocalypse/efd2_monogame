using System;
using SharpECS;

namespace EfD2.Components
{
	public class Movable : IComponent
	{
		public Entity Owner { get; set; }

		public float MoveSpeed { get; set; } = 700;
		public MoveDirection CurrentDirection { get; set; } = MoveDirection.None;
		public MoveDirection PreviousDirection { get; set; } = MoveDirection.None;
		public float Acceleration { get; set; } = 0;
		public float MaxAcceleration { get; } = 1;

		public Movable()
		{
		}
	}
}
