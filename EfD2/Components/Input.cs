using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	public class Input : IComponent
	{
		Entity IComponent.entity { get; set; }

		public InputType Type = InputType.Direct;
		public List<InputValue> CurrentInput;

		public Input()
		{
			CurrentInput = new List<InputValue>();
		}
	}
}
