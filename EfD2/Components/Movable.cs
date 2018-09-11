using System;
using ECS;

namespace EfD2.Components
{
	public class Movable : IComponent
	{
		Entity IComponent.entity { get; set; }

		public float MoveSpeed { get; set; } = 700;
		public Direction CurrentDirection { get; set; } = Direction.None;
		public Direction PreviousDirection { get; set; } = Direction.None;
		public float Acceleration { get; set; } = 0;
		public float MaxAcceleration { get; } = 1;
		public float Mass { get; set; } = 1;

		public Movable()
		{
		}
	}
}
