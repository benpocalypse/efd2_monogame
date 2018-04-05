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
	internal class GameLogicSystem
		: EntitySystem
	{
		private EntityPool entityPool;

		public GameLogicSystem(EntityPool pool)
			: base(pool, typeof(Positionable), typeof(Collidable))
		{
			entityPool = pool;
		}
	}
}
