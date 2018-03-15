using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using EfD2.Components;

namespace EfD2.Systems
{
	internal class InputSystem 
		: EntitySystem
	{
		public InputSystem(EntityPool pool)
			: base(pool, typeof(Drawable), typeof(Positionable), typeof(Movable))
        { }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < Compatible.Count; i++)
            {
                if (Compatible[i].State == EntityState.Active)
                {
                    var position = Compatible[i].GetComponent<Positionable>();
                    var moveSpeed = Compatible[i].GetComponent<Movable>().MoveSpeed;

                    if (Keyboard.GetState().IsKeyDown(Keys.D)) { position.SetX(position.Position.X + moveSpeed * delta); }
                    if (Keyboard.GetState().IsKeyDown(Keys.A)) { position.SetX(position.Position.X - moveSpeed * delta); }
                    if (Keyboard.GetState().IsKeyDown(Keys.W)) { position.SetY(position.Position.Y - moveSpeed * delta); }
                    if (Keyboard.GetState().IsKeyDown(Keys.S)) { position.SetY(position.Position.Y + moveSpeed * delta); }
                }
            }
        }
	}
}
