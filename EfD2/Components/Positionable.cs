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

        public enum PositionModeEnum
        {
            Absolute,
            Relative
        }

        private Vector2 _currentPosition;
        public Vector2 CurrentPosition
        {
            get { return _currentPosition; }
            set { _currentPosition = value; }
        }

        private Vector2 _previousPosition;
        public Vector2 PreviousPosition
        {
            get { return _previousPosition; }
            set { _previousPosition = value; }
        }

        private Vector2 _relativePosition;

        public Vector2 RelativePosition
        {
            get { return _relativePosition; }
            set { _relativePosition = value; }
        }

        private PositionModeEnum _positionMode = PositionModeEnum.Absolute;
        public PositionModeEnum PositionMode
        {
            get { return _positionMode; }
            set { _positionMode = value; }
        }



        public Entity RelativeReference = null;

        public float ZOrder = 0.0f;

        public void SetX(float newX) => _currentPosition.X = newX;
        public void SetY(float newY) => _currentPosition.Y = newY;

        public Positionable()
        {
        }
    }
}
