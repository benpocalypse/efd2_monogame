using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECS;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EfD2.Components
{
	internal class Positionable : IComponent
	{
		Entity IComponent.entity { get; set; }
        
		private Vector2 _currentPosition;

        public Vector2 CurrentPosition
        {
            get { return _currentPosition; }
            set { _currentPosition = value; }
        }

		private Vector2 _previouisPosition;

		public Vector2 PreviousPosition
		{
			get { return _previouisPosition; }
			set { _previouisPosition = value; }
		}

		public float ZOrder = 0.0f;

        public void SetX(float newX) =>_currentPosition.X = newX;
        public void SetY(float newY) => _currentPosition.Y = newY;

        public Positionable()
        {
        }
	}
}
