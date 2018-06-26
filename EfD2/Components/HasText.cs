using System.Collections.Generic;
using ECS;

namespace EfD2
{
	internal class HasText : IComponent
	{
		Entity IComponent.entity { get; set; }
		public List<string> Text;
		public bool TakesFocus = false;
		public bool Border = false;
		public bool RequiresAcknowledgment = false;
		public DisplayLayer ZOrder = DisplayLayer.Text;

		public HasText()
		{
			Text = new List<string>();
		}
	}
}
