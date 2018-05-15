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

		private Dictionary<string, Texture2D> fontDictionary = new Dictionary<string, Texture2D>(68);
		private ContentManager Content;

		public DrawingSystem(ref ContentManager _content)
		{
			Content = _content;

			// Add our numbers
			for (int i = 0; i < 10; i++)
			{
				fontDictionary.Add(i.ToString(), Content.Load<Texture2D>("font/" + i.ToString()));
				 
			}

			fontDictionary.Add(":", Content.Load<Texture2D>("font/colon"));
			fontDictionary.Add("@", Content.Load<Texture2D>("font/copyright"));
			fontDictionary.Add(",", Content.Load<Texture2D>("font/comma"));
			fontDictionary.Add("-", Content.Load<Texture2D>("font/dash"));
			fontDictionary.Add(".", Content.Load<Texture2D>("font/period"));
			fontDictionary.Add("!", Content.Load<Texture2D>("font/!"));
			fontDictionary.Add(" ", Content.Load<Texture2D>("black"));

			fontDictionary.Add("A", Content.Load<Texture2D>("font/aa"));
			fontDictionary.Add("B", Content.Load<Texture2D>("font/bb"));
			fontDictionary.Add("C", Content.Load<Texture2D>("font/cc"));
			fontDictionary.Add("D", Content.Load<Texture2D>("font/dd"));
			fontDictionary.Add("E", Content.Load<Texture2D>("font/ee"));
			fontDictionary.Add("F", Content.Load<Texture2D>("font/ff"));
			fontDictionary.Add("G", Content.Load<Texture2D>("font/gg"));
			fontDictionary.Add("H", Content.Load<Texture2D>("font/hh"));
			fontDictionary.Add("I", Content.Load<Texture2D>("font/ii"));
			fontDictionary.Add("J", Content.Load<Texture2D>("font/jj"));
			fontDictionary.Add("K", Content.Load<Texture2D>("font/kk"));
			fontDictionary.Add("L", Content.Load<Texture2D>("font/ll"));
			fontDictionary.Add("M", Content.Load<Texture2D>("font/mm"));
			fontDictionary.Add("N", Content.Load<Texture2D>("font/nn"));
			fontDictionary.Add("O", Content.Load<Texture2D>("font/oo"));
			fontDictionary.Add("P", Content.Load<Texture2D>("font/pp"));
			fontDictionary.Add("Q", Content.Load<Texture2D>("font/qq"));
			fontDictionary.Add("R", Content.Load<Texture2D>("font/rr"));
			fontDictionary.Add("S", Content.Load<Texture2D>("font/ss"));
			fontDictionary.Add("T", Content.Load<Texture2D>("font/tt"));
			fontDictionary.Add("U", Content.Load<Texture2D>("font/uu"));
			fontDictionary.Add("V", Content.Load<Texture2D>("font/vv"));
			fontDictionary.Add("W", Content.Load<Texture2D>("font/ww"));
			fontDictionary.Add("X", Content.Load<Texture2D>("font/xx"));
			fontDictionary.Add("Y", Content.Load<Texture2D>("font/yy"));
			fontDictionary.Add("Z", Content.Load<Texture2D>("font/zz"));

			fontDictionary.Add("a", Content.Load<Texture2D>("font/a"));
			fontDictionary.Add("b", Content.Load<Texture2D>("font/b"));
			fontDictionary.Add("c", Content.Load<Texture2D>("font/c"));
			fontDictionary.Add("d", Content.Load<Texture2D>("font/d"));
			fontDictionary.Add("e", Content.Load<Texture2D>("font/e"));
			fontDictionary.Add("f", Content.Load<Texture2D>("font/f"));
			fontDictionary.Add("g", Content.Load<Texture2D>("font/g"));
			fontDictionary.Add("h", Content.Load<Texture2D>("font/h"));
			fontDictionary.Add("i", Content.Load<Texture2D>("font/i"));
			fontDictionary.Add("j", Content.Load<Texture2D>("font/j"));
			fontDictionary.Add("k", Content.Load<Texture2D>("font/k"));
			fontDictionary.Add("l", Content.Load<Texture2D>("font/l"));
			fontDictionary.Add("m", Content.Load<Texture2D>("font/m"));
			fontDictionary.Add("n", Content.Load<Texture2D>("font/n"));
			fontDictionary.Add("o", Content.Load<Texture2D>("font/o"));
			fontDictionary.Add("p", Content.Load<Texture2D>("font/p"));
			fontDictionary.Add("q", Content.Load<Texture2D>("font/q"));
			fontDictionary.Add("r", Content.Load<Texture2D>("font/r"));
			fontDictionary.Add("s", Content.Load<Texture2D>("font/s"));
			fontDictionary.Add("t", Content.Load<Texture2D>("font/t"));
			fontDictionary.Add("u", Content.Load<Texture2D>("font/u"));
			fontDictionary.Add("v", Content.Load<Texture2D>("font/v"));
			fontDictionary.Add("w", Content.Load<Texture2D>("font/w"));
			fontDictionary.Add("x", Content.Load<Texture2D>("font/x"));
			fontDictionary.Add("y", Content.Load<Texture2D>("font/y"));
			fontDictionary.Add("z", Content.Load<Texture2D>("font/z"));
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

		public void Animate(SpriteBatch spriteBatch, GameTime gameTime)
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

		public void DrawText(string textToDraw, SpriteBatch spriteBatch, int x, int y)
		{
			Vector2 v = new Vector2(x*8, y*8);

			foreach(char c in textToDraw)
			{
				if (c == '\n')
				{
					v.Y += 8;
					v.X = x * 8;
				}
				else
				{
					var tex = fontDictionary.FirstOrDefault(_ => _.Key == c.ToString());

					if (tex.Value != null)
					{
						spriteBatch.Draw(tex.Value, v, null, Color.White, 0f, new Vector2(tex.Value.Width / 2, tex.Value.Height / 2), Vector2.One, SpriteEffects.None, 1.0f);
						v.X += 8;
					}
				}
			}
		}
	}
}
