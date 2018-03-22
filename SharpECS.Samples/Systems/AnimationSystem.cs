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
	internal class AnimationSystem 
		: EntitySystem
	{
		public AnimationSystem(EntityPool pool)
			: base(pool, typeof(Positionable), typeof(Drawable))
		{ }

		private float frameCounter = 0;

		public void Animate(SpriteBatch spriteBatch, GameTime gameTime)
        {
			var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			frameCounter += delta;

			for (int i = 0; i < Compatible.Count; i++)
            {
                var position = Compatible[i].GetComponent<Positionable>();
                var animatable = Compatible[i].GetComponent<Drawable>();

				foreach(Animation a in animatable.AnimationList)
				{
					if (a.Type == animatable.Type)
					{
						switch (a.Type)
						{
							case AnimationType.None:
								if (frameCounter >= a.FrameSpeed)
								{
									frameCounter = 0;
									a.CurrentFrame = (++a.CurrentFrame) % (a.FrameList.Count);
								}

								position.Rect = new RectangleF(position.CurrentPosition.X, position.CurrentPosition.Y, a.FrameList[a.CurrentFrame].Width, a.FrameList[a.CurrentFrame].Height);

								if (Compatible[i].State == EntityState.Active)
								{
									var texture = a.FrameList[a.CurrentFrame];
									var pos = position.CurrentPosition;

									spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
								}
								break;

							case AnimationType.Idle:
								if (frameCounter >= a.FrameSpeed)
								{
									frameCounter = 0;
									a.CurrentFrame = (++a.CurrentFrame) % (a.FrameList.Count);
								}

								position.Rect = new RectangleF(position.CurrentPosition.X, position.CurrentPosition.Y, a.FrameList[a.CurrentFrame].Width, a.FrameList[a.CurrentFrame].Height);

								if (Compatible[i].State == EntityState.Active)
								{
									var texture = a.FrameList[a.CurrentFrame];
									var pos = position.CurrentPosition;

									spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
								}
								break;

							case AnimationType.Running:
								if (frameCounter >= a.FrameSpeed)
								{
									frameCounter = 0;
									a.CurrentFrame = (++a.CurrentFrame) % (a.FrameList.Count);
								}

								position.Rect = new RectangleF(position.CurrentPosition.X, position.CurrentPosition.Y, a.FrameList[a.CurrentFrame].Width, a.FrameList[a.CurrentFrame].Height);

								if (Compatible[i].State == EntityState.Active)
								{
									var texture = a.FrameList[a.CurrentFrame];
									var pos = position.CurrentPosition;

									spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
								}
								break;

							case AnimationType.Attacking:
								if (frameCounter >= a.FrameSpeed)
								{
									frameCounter = 0;
									a.CurrentFrame = (++a.CurrentFrame) % (a.FrameList.Count);
								}

								position.Rect = new RectangleF(position.CurrentPosition.X, position.CurrentPosition.Y, a.FrameList[a.CurrentFrame].Width, a.FrameList[a.CurrentFrame].Height);

								if (Compatible[i].State == EntityState.Active)
								{
									var texture = a.FrameList[a.CurrentFrame];
									var pos = position.CurrentPosition;

									spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
								}
								break;
						}
					}
				}
            }
		}
	}
}
