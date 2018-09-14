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
	public class HUDSystem
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		private Texture2D hudCorner;
		private Texture2D hudHorizontal;
		private Texture2D hudVertical;

		private ContentManager contentManager;
		private SpriteBatch spriteBatch;

		Entity goldHUDTextEntity;
		Entity goldHUDValueEntity;
		HasText goldHUDValueText;
	
		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(GameState)); }
		}

		public Filter inventoryFilterMatch
		{
			get { return new Filter().AllOf(typeof(Inventory)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public HUDSystem(ref ContentManager _content, ref SpriteBatch _spriteBatch)
		{
			contentManager = _content;
			spriteBatch = _spriteBatch;

			hudCorner = contentManager.Load<Texture2D>("hud_corner");
			hudVertical = contentManager.Load<Texture2D>("hud_vertical");
			hudHorizontal = contentManager.Load<Texture2D>("hud_horizontal");

			goldHUDTextEntity = new Entity("Gold HUD");
			goldHUDTextEntity.AddComponent(new Positionable { CurrentPosition = new Vector2(1, 2), ZOrder = (float)DisplayLayer.Text });
			HasText t = new HasText();
			t.Text.Add("Gold:");
			t.Homgeneous = false;
			t.Border = false;
			goldHUDTextEntity.AddComponent(t);

			goldHUDValueEntity = new Entity("Gold HUD Value");
			goldHUDValueText = new HasText();
			goldHUDValueText.Text.Add("0");
			goldHUDValueText.Homgeneous = false;
			goldHUDValueText.Border = false;
			goldHUDValueEntity.AddComponent(goldHUDValueText);
			goldHUDValueEntity.AddComponent(new Positionable { CurrentPosition = new Vector2(7, 2), ZOrder = (float)DisplayLayer.Text });
		}

		public void Update(GameTime gameTime)
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
			spriteBatch.Draw(hudCorner, new Vector2(0,     0), Color.White);
			spriteBatch.Draw(hudCorner, new Vector2(31*8,  0), Color.White);

			spriteBatch.Draw(hudCorner, new Vector2(0,      4 * 8), Color.White);
			spriteBatch.Draw(hudCorner, new Vector2(31 * 8, 4 * 8), Color.White);

			spriteBatch.Draw(hudCorner, new Vector2(0,      27 * 8), Color.White);
			spriteBatch.Draw(hudCorner, new Vector2(31 * 8, 27 * 8), Color.White);

			// Now draw text values
			foreach (Entity e in EntityMatcher.GetMatchedEntities(inventoryFilterMatch))
			{
				goldHUDValueText.Text = new List<string>();
				goldHUDValueText.Text.Add(e.GetComponent<Inventory>().Gold.ToString());
			}
		}
	}
}
