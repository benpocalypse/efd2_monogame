using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using EfD2.Components;

namespace SharpECS.Samples
{
	internal class CollisionSystem
		: EntitySystem
	{
		public CollisionSystem(EntityPool pool)
	        : base(pool, typeof(Positionable), typeof(Collidable))
	    { }

		public void Draw(SpriteBatch spriteBatch)
	    {
			foreach(Entity in Compatible)
			{
			}

			for (int i = 0; i < Compatible.Count; i++)
	        {
	            var position = Compatible[i].GetComponent<Positionable>();
	            var drawable = Compatible[i].GetComponent<Drawable>();

	            position.Rect = new Rectangle((int)position.Position.X, (int)position.Position.Y, drawable.Texture.Width, drawable.Texture.Height);

	            if (Compatible[i].State == EntityState.Active)
	            {
	                var texture = drawable.Texture;
	                var pos = position.Position;

					spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
	            }
	        }
		}
	}
}
