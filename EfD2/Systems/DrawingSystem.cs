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

		private const bool DEBUG = false;
		private Texture2D dummyTexture;
		private Color Colori;

		// At the top of your class:
		private Texture2D pixel;
		private int Odd = 0;

		private ContentManager contentManager;
		private SpriteBatch spriteBatch;

		public DrawingSystem(ref ContentManager _content, ref SpriteBatch _spriteBatch)
		{
			contentManager = _content;
			spriteBatch = _spriteBatch;

			dummyTexture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
			dummyTexture.SetData(new Color[] { Color.White });

			pixel = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			pixel.SetData(new[] { Color.White });

			Colori = Color.White;
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

						//position.Rect = new RectangleF(position.CurrentPosition.X, position.CurrentPosition.Y, a.FrameList[a.CurrentFrame].Width, a.FrameList[a.CurrentFrame].Height);

						if (DEBUG == true)
						{
							var openSpot = EntityMatcher.GetEntity("OpenSpaceNextToEntrance");
							if (openSpot != null)
							{
								var openPos = openSpot.GetComponent<Positionable>();

								var newRect = new RectangleF(openPos.CurrentPosition.X, openPos.CurrentPosition.Y, 8, 8);
								DrawBorder(newRect.ToRectangle());
							}
						}

						var texture = a.FrameList[a.CurrentFrame];						
						var pos = position.CurrentPosition;

						if (animatable.FlipOnXAxis == false) // new Vector2(texture.Width, texture.Height)
							spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.One, Vector2.One, SpriteEffects.None, (float)animatable.ZOrder / (float)DisplayLayer.MAX_LAYER);
						else
							spriteBatch.Draw(texture, pos, null, Color.White, 0f, Vector2.One, Vector2.One, SpriteEffects.FlipHorizontally, (float)animatable.ZOrder / (float)DisplayLayer.MAX_LAYER);
					}
				}
            }
		}

		private void DrawBorder(Rectangle rectangleToDraw)
		{
			// Draw top line
			spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, 1), Color.White);

			// Draw left line
			spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, 1, rectangleToDraw.Height), Color.White);

			// Draw right line
			spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - 1),
											rectangleToDraw.Y,
											1,
											rectangleToDraw.Height), Color.White);
			// Draw bottom line
			spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
											rectangleToDraw.Y + rectangleToDraw.Height - 1,
											rectangleToDraw.Width,
											1), Color.White);
		}
	}
}
