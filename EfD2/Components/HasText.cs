using System.Collections.Generic;
using ECS;

namespace EfD2
{
	internal class HasText : IComponent
	{
		Entity IComponent.entity { get; set; }
		public List<string> Text;
		public bool Border = false;
		public bool Homgeneous = true;
		public int CurrentText = 0;
		public DisplayLayer ZOrder = DisplayLayer.Text;

		public HasText()
		{
			Text = new List<string>();
		}
	}
}
