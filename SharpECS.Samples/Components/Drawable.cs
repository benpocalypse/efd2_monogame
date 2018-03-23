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
		public bool FlipOnXAxis = false;
		public bool FlipOnYAxis = false;

		public Drawable()
		{
			AnimationList = new List<Animation>();
		}

		public Drawable(params Animation[] aList)
		{
			AnimationList = new List<Animation>();

			foreach(Animation a in aList)
			{
				AnimationList.Add(a);
			}
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
