using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;

namespace EfD2.Systems
{
	public class InputSystem 
		: IReactiveSystem
	{
		public bool isTriggered { get { return receivedEntity != null; } }
		public Entity receivedEntity;

		public Filter filterMatch
		{
			get { return new Filter().AllOf(typeof(HasInput)); }
		}

		public void Execute(Entity modifiedEntity)
		{
			receivedEntity = modifiedEntity;
		}

		public InputSystem()
        { 
		}

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var input = e.GetComponent<HasInput>();

				// FIXME - Might not want to clear this every time
				input.CurrentInput.Clear();

				if (Keyboard.GetState().IsKeyDown(Keys.D))
				{
					input.CurrentInput.Add(Input.Right);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.A))
				{
					input.CurrentInput.Add(Input.Left);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.W))
				{
					input.CurrentInput.Add(Input.Up);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.S))
				{
					input.CurrentInput.Add(Input.Down);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.J))
				{
					input.CurrentInput.Add(Input.A);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.K))
				{
					input.CurrentInput.Add(Input.B);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.Space))
				{
					input.CurrentInput.Add(Input.Start);
				}

				if (Keyboard.GetState().IsKeyDown(Keys.Enter))
				{
					input.CurrentInput.Add(Input.Select);
				}
			}
        }
	}
}
