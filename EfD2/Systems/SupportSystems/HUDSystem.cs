using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;
using Microsoft.Xna.Framework.Content;

namespace EfD2.Systems.SupportSystems
{
	public class HudSystem
	{
		private Texture2D hudCorner;
		private Texture2D hudHorizontal;
		private Texture2D hudVertical;

		private Texture2D heartFull;
		private Texture2D heartEmpty;

		private ContentManager contentManager;
		private SpriteBatch spriteBatch;

		Entity goldHUDTextEntity;
		Entity goldHUDValueEntity;
		Text goldHUDValueText;

		Entity lifeHUDTextEntity;
	
		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(GameState)); }
		}

		public Filter inventoryFilterMatch
		{
			get { return new Filter().AllOf(typeof(Inventory)); }
		}

		public Filter healthFilterMatch
		{
			get { return new Filter().AllOf(typeof(Health)); }
		}

		public HudSystem(ref ContentManager _content, ref SpriteBatch _spriteBatch)
		{
			contentManager = _content;
			spriteBatch = _spriteBatch;

			hudCorner = contentManager.Load<Texture2D>("hud_corner");
			hudVertical = contentManager.Load<Texture2D>("hud_vertical");
			hudHorizontal = contentManager.Load<Texture2D>("hud_horizontal");

			heartFull = contentManager.Load<Texture2D>("heart_full");
			heartEmpty = contentManager.Load<Texture2D>("heart_empty");

			goldHUDTextEntity = new Entity("Gold HUD");
			goldHUDTextEntity.AddComponent(new Positionable { CurrentPosition = new Vector2(2, 2), ZOrder = (float)DisplayLayer.Text });
			Text t1 = new Text();
			t1.TextList.Add("Gold:");
			t1.Homgeneous = false;
			t1.Border = false;
			goldHUDTextEntity.AddComponent(t1);

			goldHUDValueEntity = new Entity("Gold HUD Value");
			goldHUDValueText = new Text();
			goldHUDValueText.TextList.Add("0");
			goldHUDValueText.Homgeneous = false;
			goldHUDValueText.Border = false;
			goldHUDValueEntity.AddComponent(goldHUDValueText);
			goldHUDValueEntity.AddComponent(new Positionable { CurrentPosition = new Vector2(9, 2), ZOrder = (float)DisplayLayer.Text });

			lifeHUDTextEntity = new Entity("Life HUD Text");
			lifeHUDTextEntity.AddComponent(new Positionable() { CurrentPosition = new Vector2(2, 1), ZOrder = (float)DisplayLayer.Text });
			Text t2 = new Text();
			t2.TextList.Add("Life:");
			t2.Homgeneous = false;
			t2.Border = false;
			lifeHUDTextEntity.AddComponent(t2);
		}

		public void Update(GameTime gameTime)
		{
			// Only show the HUD while we're playing
			if (EntityMatcher.GetMatchedEntities(filterMatch).First().GetComponent<GameState>().CurrentState == GameStateType.Playing)
			{
				// Draw Verticals
				for (int i = 1; i < 27; i++)
				{
					if (i != 4)
					{
						spriteBatch.Draw(hudVertical, new Vector2(0, i * 8), Color.White);
						spriteBatch.Draw(hudVertical, new Vector2(31 * 8, i * 8), Color.White);
					}
				}

				// Draw Horizontals
				for (int i = 1; i < 31; i++)
				{
					spriteBatch.Draw(hudHorizontal, new Vector2((i * 8), 0), Color.White);
					spriteBatch.Draw(hudHorizontal, new Vector2((i * 8), 4 * 8), Color.White);
					spriteBatch.Draw(hudHorizontal, new Vector2((i * 8), 27 * 8), Color.White);
				}

				// Draw Corners
				spriteBatch.Draw(hudCorner, new Vector2(0, 0), Color.White);
				spriteBatch.Draw(hudCorner, new Vector2(31 * 8, 0), Color.White);

				spriteBatch.Draw(hudCorner, new Vector2(0, 4 * 8), Color.White);
				spriteBatch.Draw(hudCorner, new Vector2(31 * 8, 4 * 8), Color.White);

				spriteBatch.Draw(hudCorner, new Vector2(0, 27 * 8), Color.White);
				spriteBatch.Draw(hudCorner, new Vector2(31 * 8, 27 * 8), Color.White);

				// Draw health
				foreach (Entity e in EntityMatcher.GetMatchedEntities(healthFilterMatch))
				{
					for (int i = 0; i < e.GetComponent<Health>().Max; i++)
					{
						if (e.GetComponent<Health>().Value > i)
							spriteBatch.Draw(heartFull, new Vector2((7 * 8) + (i*8), 8), Color.White);
						else
							spriteBatch.Draw(heartEmpty, new Vector2((7 * 8) + (i*8), 8), Color.White);
					}
					     
				}

				// Now draw text values
				foreach (Entity e in EntityMatcher.GetMatchedEntities(inventoryFilterMatch))
				{
					goldHUDValueText.TextList[0] = e.GetComponent<Inventory>().Gold.ToString();
				}
			}
		}
	}
}
