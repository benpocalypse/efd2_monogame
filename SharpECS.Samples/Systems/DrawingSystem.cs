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
			: base(pool, typeof(Positionable), typeof(Drawable))
		{ }

		public void Animate(SpriteBatch spriteBatch, GameTime gameTime)
        {
			var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;


			for (int i = 0; i < Compatible.Count; i++)
            {
                var position = Compatible[i].GetComponent<Positionable>();
                var animatable = Compatible[i].GetComponent<Drawable>();

				foreach(Animation a in animatable.AnimationList)
				{
					if (a.Type == animatable.Type)
					{
						a.FrameCounter += delta;

						if (a.FrameCounter >= a.FrameSpeed)
						{
							a.FrameCounter = 0;
							a.CurrentFrame = (++a.CurrentFrame) % (a.FrameList.Count);
						}

						position.Rect = new RectangleF(position.CurrentPosition.X, position.CurrentPosition.Y, a.FrameList[a.CurrentFrame].Width, a.FrameList[a.CurrentFrame].Height);

						if (Compatible[i].State == EntityState.Active)
						{
							var texture = a.FrameList[a.CurrentFrame];
							
							var pos = position.CurrentPosition;

							if(animatable.FlipOnXAxis == false)
								spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, position.ZOrder);
							else
								spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.FlipHorizontally, position.ZOrder);
						}
					}
				}
            }
		}
	}
}
