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
			get { return new Filter().AllOf(typeof(Positionable), typeof(Collidable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public GameSystem()
		{
			
		}

		/*
		public GameSystem(EntityPool pool)
			: base(pool, typeof(Positionable), typeof(Collidable))
		{
			entityPool = pool;
		}
		*/
	}
}
