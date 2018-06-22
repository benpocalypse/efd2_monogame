using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;
using Microsoft.Xna.Framework.Content;

namespace EfD2.Systems
{
	public class DrawingSystem 
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;


		private ContentManager contentManager;
		private SpriteBatch spriteBatch;

		public DrawingSystem(ref ContentManager _content, ref SpriteBatch _spriteBatch)
		{
			contentManager = _content;
			spriteBatch = _spriteBatch;
		}

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(Positionable), typeof(Drawable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public void DrawIntro(SpriteBatch spriteBatch, GameTime gameTime)
		{
			var introTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

			//spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, position.ZOrder);
		}

		public void Animate(GameTime gameTime)
        {
			var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			foreach(Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
            {
                var position = e.GetComponent<Positionable>();
                var animatable = e.GetComponent<Drawable>();

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
