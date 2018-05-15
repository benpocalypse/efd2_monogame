using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;

namespace EfD2.Systems
{
	internal class GameSystem 
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		//private EntityPool entityPool;

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(GameStatable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public GameSystem()
		{
			
		}

		public void Update(GameTime gameTime)
		{
			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var state = e.GetComponent<GameStatable>();
			}
		}
	}
}
