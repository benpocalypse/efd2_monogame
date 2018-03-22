using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpECS;
using EfD2;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections;

namespace EfD2.Components
{
	internal class Drawable : IComponent, IEnumerable
	{
		public List<Animation> AnimationList;
		public AnimationType Type = AnimationType.None;

		public Drawable()
		{
			AnimationList = new List<Animation>();
		}

		public void AddAnimation(Animation a)
		{
			AnimationList.Add(a);
		}

		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
