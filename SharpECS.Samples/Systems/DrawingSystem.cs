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
	internal class DrawingSystem 
		: EntitySystem
	{
		public DrawingSystem(EntityPool pool)
            : base(pool, typeof(Drawable), typeof(Positionable))
        { }

		public void Draw(SpriteBatch spriteBatch)
        {
			for (int i = 0; i < Compatible.Count; i++)
            {
                var position = Compatible[i].GetComponent<Positionable>();
                var drawable = Compatible[i].GetComponent<Drawable>();

                position.Rect = new RectangleF(position.CurrentPosition.X, position.CurrentPosition.Y, drawable.AnimationList[0].FrameList[0].Width, drawable.AnimationList[0].FrameList[0].Height);

                if (Compatible[i].State == EntityState.Active)
                {
                    var texture = drawable.AnimationList[0].FrameList[0];
                    var pos = position.CurrentPosition;

					spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                }
            }
		}
	}
}
