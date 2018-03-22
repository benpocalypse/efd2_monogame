using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using EfD2.Components;

namespace EfD2.Systems
{
	internal class CollisionSystem
		: EntitySystem
	{
		public CollisionSystem(EntityPool pool)
			: base(pool, typeof(Positionable), typeof(Collidable), typeof(Drawable))
		{ }

		public void Update(GameTime gameTime)
		{
			foreach (Entity e in Compatible.Where(_ => _.State == EntityState.Active))
			{
				var pos1 = e.GetComponent<Positionable>();
				var col1 = e.GetComponent<Collidable>();
				Drawable draw1 = e.GetComponent<Drawable>();

				pos1.Rect = new RectangleF(pos1.CurrentPosition.X, pos1.CurrentPosition.Y, draw1.AnimationList[0].FrameList[0].Width, draw1.AnimationList[0].FrameList[0].Height);

				foreach (Entity o in Compatible.Where(_ => _.State == EntityState.Active && !_.Equals(e)))
				{
					var pos2 = o.GetComponent<Positionable>();
					var col2 = o.GetComponent<Collidable>();
					Drawable draw2 = o.GetComponent<Drawable>();

					pos2.Rect = new RectangleF(pos2.CurrentPosition.X, pos2.CurrentPosition.Y, draw2.AnimationList[0].FrameList[0].Width, draw2.AnimationList[0].FrameList[0].Height);

					if (pos1.Rect.Intersects(pos2.Rect))
					{
						col1.Colliding = true;
						col2.Colliding = true;

						if(!col1.CollidingEntities.Contains(o))
							col1.CollidingEntities.Add(o);

						if (!col2.CollidingEntities.Contains(e))
							col2.CollidingEntities.Add(e);
					}
				}
			}
		}
	}
}
