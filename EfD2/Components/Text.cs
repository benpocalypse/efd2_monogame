using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
	internal class Text : IComponent
	{
		Entity IComponent.entity { get; set; }
		public List<string> TextList;
		public bool Border = false;
		public bool Homgeneous = true;
		public int CurrentText = 0;
		public DisplayLayer ZOrder = DisplayLayer.Text;

		public Text()
		{
			TextList = new List<string>();
		}
	}
}
