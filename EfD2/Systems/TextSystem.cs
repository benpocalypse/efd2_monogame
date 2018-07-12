using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;
using Microsoft.Xna.Framework.Content;

namespace EfD2
{
	public class TextSystem : IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		private ContentManager contentManager;
		private SpriteBatch spriteBatch;
		private Dictionary<string, Texture2D> fontDictionary = new Dictionary<string, Texture2D>(68);
		private Texture2D[] borderArray;

		public TextSystem(ref ContentManager _content, ref SpriteBatch _spriteBatch)
		{
			contentManager = _content;
			spriteBatch = _spriteBatch;

			// Add our numbers
			for (int i = 0; i < 10; i++)
			{
				fontDictionary.Add(i.ToString(), contentManager.Load<Texture2D>("font/" + i.ToString()));

			}

			fontDictionary.Add(":", contentManager.Load<Texture2D>("font/colon"));
			fontDictionary.Add("@", contentManager.Load<Texture2D>("font/copyright"));
			fontDictionary.Add(",", contentManager.Load<Texture2D>("font/comma"));
			fontDictionary.Add("-", contentManager.Load<Texture2D>("font/dash"));
			fontDictionary.Add(".", contentManager.Load<Texture2D>("font/period"));
			fontDictionary.Add("!", contentManager.Load<Texture2D>("font/!"));
			fontDictionary.Add("?", contentManager.Load<Texture2D>("font/question"));
			fontDictionary.Add(" ", contentManager.Load<Texture2D>("black"));

			fontDictionary.Add("A", contentManager.Load<Texture2D>("font/aa"));
			fontDictionary.Add("B", contentManager.Load<Texture2D>("font/bb"));
			fontDictionary.Add("C", contentManager.Load<Texture2D>("font/cc"));
			fontDictionary.Add("D", contentManager.Load<Texture2D>("font/dd"));
			fontDictionary.Add("E", contentManager.Load<Texture2D>("font/ee"));
			fontDictionary.Add("F", contentManager.Load<Texture2D>("font/ff"));
			fontDictionary.Add("G", contentManager.Load<Texture2D>("font/gg"));
			fontDictionary.Add("H", contentManager.Load<Texture2D>("font/hh"));
			fontDictionary.Add("I", contentManager.Load<Texture2D>("font/ii"));
			fontDictionary.Add("J", contentManager.Load<Texture2D>("font/jj"));
			fontDictionary.Add("K", contentManager.Load<Texture2D>("font/kk"));
			fontDictionary.Add("L", contentManager.Load<Texture2D>("font/ll"));
			fontDictionary.Add("M", contentManager.Load<Texture2D>("font/mm"));
			fontDictionary.Add("N", contentManager.Load<Texture2D>("font/nn"));
			fontDictionary.Add("O", contentManager.Load<Texture2D>("font/oo"));
			fontDictionary.Add("P", contentManager.Load<Texture2D>("font/pp"));
			fontDictionary.Add("Q", contentManager.Load<Texture2D>("font/qq"));
			fontDictionary.Add("R", contentManager.Load<Texture2D>("font/rr"));
			fontDictionary.Add("S", contentManager.Load<Texture2D>("font/ss"));
			fontDictionary.Add("T", contentManager.Load<Texture2D>("font/tt"));
			fontDictionary.Add("U", contentManager.Load<Texture2D>("font/uu"));
			fontDictionary.Add("V", contentManager.Load<Texture2D>("font/vv"));
			fontDictionary.Add("W", contentManager.Load<Texture2D>("font/ww"));
			fontDictionary.Add("X", contentManager.Load<Texture2D>("font/xx"));
			fontDictionary.Add("Y", contentManager.Load<Texture2D>("font/yy"));
			fontDictionary.Add("Z", contentManager.Load<Texture2D>("font/zz"));

			fontDictionary.Add("a", contentManager.Load<Texture2D>("font/a"));
			fontDictionary.Add("b", contentManager.Load<Texture2D>("font/b"));
			fontDictionary.Add("c", contentManager.Load<Texture2D>("font/c"));
			fontDictionary.Add("d", contentManager.Load<Texture2D>("font/d"));
			fontDictionary.Add("e", contentManager.Load<Texture2D>("font/e"));
			fontDictionary.Add("f", contentManager.Load<Texture2D>("font/f"));
			fontDictionary.Add("g", contentManager.Load<Texture2D>("font/g"));
			fontDictionary.Add("h", contentManager.Load<Texture2D>("font/h"));
			fontDictionary.Add("i", contentManager.Load<Texture2D>("font/i"));
			fontDictionary.Add("j", contentManager.Load<Texture2D>("font/j"));
			fontDictionary.Add("k", contentManager.Load<Texture2D>("font/k"));
			fontDictionary.Add("l", contentManager.Load<Texture2D>("font/l"));
			fontDictionary.Add("m", contentManager.Load<Texture2D>("font/m"));
			fontDictionary.Add("n", contentManager.Load<Texture2D>("font/n"));
			fontDictionary.Add("o", contentManager.Load<Texture2D>("font/o"));
			fontDictionary.Add("p", contentManager.Load<Texture2D>("font/p"));
			fontDictionary.Add("q", contentManager.Load<Texture2D>("font/q"));
			fontDictionary.Add("r", contentManager.Load<Texture2D>("font/r"));
			fontDictionary.Add("s", contentManager.Load<Texture2D>("font/s"));
			fontDictionary.Add("t", contentManager.Load<Texture2D>("font/t"));
			fontDictionary.Add("u", contentManager.Load<Texture2D>("font/u"));
			fontDictionary.Add("v", contentManager.Load<Texture2D>("font/v"));
			fontDictionary.Add("w", contentManager.Load<Texture2D>("font/w"));
			fontDictionary.Add("x", contentManager.Load<Texture2D>("font/x"));
			fontDictionary.Add("y", contentManager.Load<Texture2D>("font/y"));
			fontDictionary.Add("z", contentManager.Load<Texture2D>("font/z"));

			borderArray = new Texture2D[8];
			// and finally add the borders.
			borderArray[0] = contentManager.Load<Texture2D>("font/border_top_left");
			borderArray[1] = contentManager.Load<Texture2D>("font/border_top_middle");
			borderArray[2] = contentManager.Load<Texture2D>("font/border_top_right");

			borderArray[3] = contentManager.Load<Texture2D>("font/border_left_middle");
			borderArray[4] = contentManager.Load<Texture2D>("font/border_right_middle");

			borderArray[5] = contentManager.Load<Texture2D>("font/border_bottom_left");
			borderArray[6] = contentManager.Load<Texture2D>("font/border_bottom_middle");
			borderArray[7] = contentManager.Load<Texture2D>("font/border_bottom_right");
		}

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(Ephemeral), typeof(HasText), typeof(Positionable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public void Update(GameTime gameTime)
		{
			var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var ephemeral = e.GetComponent<Ephemeral>();
				var text = e.GetComponent<HasText>();
				var pos = e.GetComponent<Positionable>();

				ephemeral.PersistTime -= delta;

				if ((ephemeral.PersistTime >= 0.0) && (text.CurrentText < text.Text.Count))
				{
					var current = text.Text[text.CurrentText];
					DrawText(current, text, pos);

					if (text.Border == true)
						DrawBorder(current, text, pos);
				}
				else
				{
					if (text.CurrentText < text.Text.Count)
					{
						var current = text.Text[text.CurrentText];
						DrawText(current, text, pos);

						if (text.Border == true)
							DrawBorder(current, text, pos);
						
						text.CurrentText++;
						ephemeral.PersistTime = ephemeral.TotalTime;
					}
				}
			}
		}

		private void DrawText(string currentText, HasText text, Positionable pos)
		{
			Vector2 v = new Vector2(pos.CurrentPosition.X * 8, pos.CurrentPosition.Y * 8);

			foreach (char c in currentText)
			{
				if (c == '\n')
				{
					v.Y += 8;
					v.X = pos.CurrentPosition.X * 8;
				}
				else
				{
					var tex = fontDictionary.FirstOrDefault(_ => _.Key == c.ToString());

					if (tex.Value != null)
					{
						spriteBatch.Draw(tex.Value, v, null, Color.White, 0f, new Vector2(tex.Value.Width / 2, tex.Value.Height / 2), Vector2.One, SpriteEffects.None, (float)text.ZOrder / (float)DisplayLayer.MAX_LAYER);
						v.X += 8;
					}
				}
			}
		}

		private void DrawBorder(string current, HasText text, Positionable pos)
		{
			Vector2 v1 = new Vector2((pos.CurrentPosition.X-1) * 8, (pos.CurrentPosition.Y-1) * 8);

			int height = 0;
			int width = 0;

			if (text.Homgeneous == true)
			{
				foreach (string s in text.Text)
				{
					int maxHeight = 0;
					int maxWidth = 0;
					var sub = s.Split('\n');

					foreach (string subS in sub)
					{
						if (subS.Length > maxWidth)
							maxWidth = subS.Length;

						maxHeight++;
					}

					if (maxHeight > height)
						height = maxHeight;

					if (maxWidth > width)
						width = maxWidth;
				}
			}
			else
			{
				var sub = current.Split('\n');
				foreach (string subS in sub)
				{
					if (subS.Length > width)
						width = subS.Length;

					height++;
				}

			}

			for (int y = 0; y <= height; y++)
			{
				if (y == 0)
				{
					Vector2 v2 = new Vector2((pos.CurrentPosition.X + width) * 8, (pos.CurrentPosition.Y - 1) * 8);
					spriteBatch.Draw(borderArray[0], v1, null, Color.White, 0f, new Vector2(borderArray[1].Width / 2, borderArray[1].Height / 2), Vector2.One, SpriteEffects.None, (float)text.ZOrder / (float)DisplayLayer.MAX_LAYER);
					spriteBatch.Draw(borderArray[2], v2, null, Color.White, 0f, new Vector2(borderArray[2].Width / 2, borderArray[2].Height / 2), Vector2.One, SpriteEffects.None, (float)text.ZOrder / (float)DisplayLayer.MAX_LAYER);
				}


				if (y == height-1)
				{
					Vector2 v3 = new Vector2((pos.CurrentPosition.X - 1) * 8, (pos.CurrentPosition.Y + height) * 8);
					Vector2 v4 = new Vector2((pos.CurrentPosition.X + width) * 8, (pos.CurrentPosition.Y + height) * 8);
					spriteBatch.Draw(borderArray[5], v3, null, Color.White, 0f, new Vector2(borderArray[1].Width / 2, borderArray[1].Height / 2), Vector2.One, SpriteEffects.None, (float)text.ZOrder / (float)DisplayLayer.MAX_LAYER);
					spriteBatch.Draw(borderArray[7], v4, null, Color.White, 0f, new Vector2(borderArray[2].Width / 2, borderArray[2].Height / 2), Vector2.One, SpriteEffects.None, (float)text.ZOrder / (float)DisplayLayer.MAX_LAYER);
				}
				
				for (int x = 1; x <= width; x++)
				{
					bool drawn = false;

					if ((x == 1) && (y > 0) && (y <= height))
					{
						drawn = true;
						Vector2 v6 = v1;

						spriteBatch.Draw(borderArray[3], v6, null, Color.White, 0f, new Vector2(borderArray[1].Width / 2, borderArray[1].Height / 2), Vector2.One, SpriteEffects.None, (float)text.ZOrder / (float)DisplayLayer.MAX_LAYER);
					}

					if ((x == width) && (y > 0) && (y <= height))
					{
						//drawn = true;
						Vector2 v6 = v1;
						v6.X += 16;

						spriteBatch.Draw(borderArray[4], v6, null, Color.White, 0f, new Vector2(borderArray[1].Width / 2, borderArray[1].Height / 2), Vector2.One, SpriteEffects.None, (float)text.ZOrder / (float)DisplayLayer.MAX_LAYER);
					}

					v1.X += 8;

					if (y == 0)
					{
						drawn = true;
						spriteBatch.Draw(borderArray[1], v1, null, Color.White, 0f, new Vector2(borderArray[1].Width / 2, borderArray[1].Height / 2), Vector2.One, SpriteEffects.None, (float)text.ZOrder / (float)DisplayLayer.MAX_LAYER);
					}

					if (y == height)
					{
						drawn = true;
						Vector2 v5 = v1;
						v5.Y = v1.Y + 8;
						spriteBatch.Draw(borderArray[6], v5, null, Color.White, 0f, new Vector2(borderArray[1].Width / 2, borderArray[1].Height / 2), Vector2.One, SpriteEffects.None, (float)text.ZOrder / (float)DisplayLayer.MAX_LAYER);
					}

					//if (drawn == false)
					{
						var tex = fontDictionary.FirstOrDefault(_ => _.Key == " ");

						if (tex.Value != null)
						{
							spriteBatch.Draw(tex.Value, v1, null, Color.White, 0f, new Vector2(tex.Value.Width / 2, tex.Value.Height / 2), Vector2.One, SpriteEffects.None, (float)DisplayLayer.TextBackground / (float)DisplayLayer.MAX_LAYER);
						}
					}
				}

				v1.X = (pos.CurrentPosition.X - 1) * 8;
				v1.Y += 8;
			}
		}
	}
}
