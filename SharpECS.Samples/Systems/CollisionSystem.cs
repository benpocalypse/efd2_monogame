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
				var draw1 = e.GetComponent<Drawable>();

				//pos1.Rect = new RectangleF(pos1.CurrentPosition.X, pos1.CurrentPosition.Y, draw1.AnimationList[0].FrameList[0].Width, draw1.AnimationList[0].FrameList[0].Height);

				if (col1.Type == EntityType.Player)
				{
					var deltaX = Math.Abs(pos1.Rect.Width - col1.BoundingBox.Width) / 2;
					//col1.BoundingBox.X = pos1.Rect.X + delta;
					var deltaY = Math.Abs(pos1.Rect.Height - col1.BoundingBox.Height) / 2;
					//col1.BoundingBox.Y = pos1.Rect.Y + delta;

					pos1.Rect = new RectangleF(pos1.CurrentPosition.X+deltaX, pos1.CurrentPosition.Y+deltaY, col1.BoundingBox.Width, col1.BoundingBox.Height);

					foreach (Entity o in Compatible.Where(_ => _.State == EntityState.Active && !_.Equals(e)))
					{
						var pos2 = o.GetComponent<Positionable>();
						var col2 = o.GetComponent<Collidable>();

						/*
						delta = Math.Abs(pos2.Rect.Width - col2.BoundingBox.Width) / 2;
						col2.BoundingBox.X = pos2.Rect.X + delta;
						delta = Math.Abs(pos2.Rect.Height - col2.BoundingBox.Height) / 2;
						col2.BoundingBox.Y = pos2.Rect.Y + delta;
						*/

						//if (col1.BoundingBox.Intersects(col2.BoundingBox))
						if (pos1.Rect.Intersects(pos2.Rect))
						{
							col1.Colliding = true;
							//col2.Colliding = true;

							if (!col1.CollidingEntities.Contains(o))
								col1.CollidingEntities.Add(o);

							//if (!col2.CollidingEntities.Contains(e))
							//	col2.CollidingEntities.Add(e);
						}
						else
						{
							col1.CollidingEntities.Remove(o);
							//col2.CollidingEntities.Remove(e);
						}
					}
				}
			}
		}
	}
}
