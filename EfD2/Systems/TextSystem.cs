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

				if (ephemeral.PersistTime >= 0.0)
				{
					foreach (string s in text.Text)
					{
						DrawText(s, pos);
					}
				}
			}
		}

		private void DrawText(string textToDraw, Positionable pos)
		{
			Vector2 v = new Vector2(pos.CurrentPosition.X * 8, pos.CurrentPosition.Y * 8);

			foreach (char c in textToDraw)
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
						spriteBatch.Draw(tex.Value, v, null, Color.White, 0f, new Vector2(tex.Value.Width / 2, tex.Value.Height / 2), Vector2.One, SpriteEffects.None, 1.0f);
						v.X += 8;
					}
				}
			}
		}

		private void DrawBorder(Positionable pos, Sizable size)
		{
		}
	}
}
