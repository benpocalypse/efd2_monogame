using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECS;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EfD2.Components
{
	internal class Sizable : IComponent
	{
		Entity IComponent.entity { get; set; }

		public RectangleF Size { get; set; }

		public Sizable()
		{
			Size = new RectangleF();
		}
	}
}

