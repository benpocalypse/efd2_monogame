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
	public class CollisionSystem
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		public CollisionSystem()
		{ 
		}

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(Positionable), typeof(Collidable), typeof(Drawable)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}


		public void Update(GameTime gameTime)
		{
			
			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var pos1 = e.GetComponent<Positionable>();
				var col1 = e.GetComponent<Collidable>();
				var draw1 = e.GetComponent<Drawable>();

				//pos1.Rect = new RectangleF(pos1.CurrentPosition.X, pos1.CurrentPosition.Y, draw1.AnimationList[0].FrameList[0].Width, draw1.AnimationList[0].FrameList[0].Height);

				if (col1.Type == EntityType.Player)
				{
					var deltaX = Math.Abs(pos1.Rect.Width - col1.BoundingBox.Width) / 2;
					//col1.BoundingBox.X = pos1.Rect.X + delta;
					var deltaY = Math.Abs(pos1.Rect.Height - col1.BoundingBox.Height) / 2;
					//col1.BoundingBox.Y = pos1.Rect.Y + delta;

					pos1.Rect = new RectangleF(pos1.CurrentPosition.X+deltaX, pos1.CurrentPosition.Y+deltaY, col1.BoundingBox.Width, col1.BoundingBox.Height);

					foreach (Entity o in EntityMatcher.GetMatchedEntities(filterMatch).Where(_ => !_.Equals(e)))
					{
						var pos2 = o.GetComponent<Positionable>();
						var col2 = o.GetComponent<Collidable>();

						if (pos1.Rect.Intersects(pos2.Rect))
						{
							col1.Colliding = true;
							//col2.Colliding = true;

							if (!col1.CollidingEntities.Contains(o))
								col1.CollidingEntities.Add(o);

							//if (!col2.CollidingEntities.Contains(e))
							//	col2.CollidingEntities.Add(e);
						}
						else
						{
							col1.CollidingEntities.Remove(o);
							//col2.CollidingEntities.Remove(e);
						}
					}
				}
			}
		}
	}
}
