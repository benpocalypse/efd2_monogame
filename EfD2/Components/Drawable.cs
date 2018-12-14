using System;
using System.Collections.Generic;

using ECS;
using System.Collections;
using EfD2.Helpers;

namespace EfD2.Components
{
    internal class Drawable : IComponent, IEnumerable
	{
		Entity IComponent.entity { get; set; }

		public List<Animation> AnimationList;
		public AnimationType Type { get; set; }
		public DisplayLayer ZOrder = DisplayLayer.Background;
		public bool FlipOnXAxis = false;
		public bool FlipOnYAxis = false;

		public Drawable()
		{
			AnimationList = new List<Animation>();
		}

		public Drawable(AnimationType type, params Animation[] aList)
		{
			Type = type;

			AnimationList = new List<Animation>();

			foreach (Animation a in aList)
			{
				AnimationList.Add(a);
			}
		}

		public Drawable(params Animation[] aList)
		{
			AnimationList = new List<Animation>();

			foreach(Animation a in aList)
			{
				AnimationList.Add(a);
			}

			Type = AnimationType.None;
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
