using System;
using System.Collections.Generic;
using SharpECS;

namespace EfD2.Components
{
	public class Statable : IComponent
	{
		public Entity Owner { get; set; }

		public PlayerStateType PlayerState { get; set;} = PlayerStateType.None;

		public Statable()
		{
		}
	}
}
