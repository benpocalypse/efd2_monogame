using System;
using System.Collections.Generic;
using ECS;

namespace EfD2
{
	public class HasInput : IComponent
	{
		Entity IComponent.entity { get; set; }

		public InputType Type = InputType.Direct;
		public List<Input> CurrentInput;

		public HasInput()
		{
			CurrentInput = new List<Input>();
		}
	}
}
