using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using EfD2.Components;

namespace EfD2.Samples
{
	internal class CollisionSystem
		: EntitySystem
	{
		public CollisionSystem(EntityPool pool)
			: base(pool, typeof(Positionable), typeof(Collidable), typeof(Drawable))
		{ }

		public void Update(GameTime gameTime)
		{
			bool collision = false;

			foreach (Entity e in Compatible.Where(_ => _.State == EntityState.Active))
			{
				var pos1 = e.GetComponent<Positionable>();
				var col1 = e.GetComponent<Collidable>();
				var draw1 = e.GetComponent<Drawable>();

				pos1.Rect = new Rectangle((int)pos1.CurrentPosition.X, (int)pos1.CurrentPosition.Y, draw1.Texture.Width, draw1.Texture.Height);

				foreach (Entity o in Compatible.Where(_ => _.State == EntityState.Active && !_.Equals(e)))
				{
					var pos2 = o.GetComponent<Positionable>();
					var col2 = o.GetComponent<Collidable>();
					var draw2 = o.GetComponent<Drawable>();

					pos2.Rect = new Rectangle((int)pos2.CurrentPosition.X, (int)pos2.CurrentPosition.Y, draw2.Texture.Width, draw2.Texture.Height);

					if (pos1.Rect.Intersects(pos2.Rect))
					{
						col1.Colliding = true;
						col2.Colliding = true;

						if(!col1.CollidingEntities.Contains(o))
							col1.CollidingEntities.Add(o);

						if (!col2.CollidingEntities.Contains(e))
							col2.CollidingEntities.Add(e);

						collision = true;
					}
				}
			}

			if (collision == true)
			{
				foreach (Entity e in Compatible)
				{
					foreach(IComponent ic in e.Components.Where(_ => _.GetType() == typeof(Collidable)))
					{
						Console.WriteLine("Colliding = " + ((Collidable)ic).Type);

						foreach(Entity c in ((Collidable)ic).CollidingEntities)
						{
							Console.WriteLine("  Entity = " + c.Id);
						}
					}
				}
			}
		}
	}
}
