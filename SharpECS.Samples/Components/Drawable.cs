using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpECS;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EfD2.Components
{
	internal class Drawable : IComponent
	{
		public Entity Owner { get; set; }

		public Texture2D Texture { get; set; }

		public Drawable()
		{
			
		}
	}
}
