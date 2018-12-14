using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace EfD2.Helpers
{
    public class Animation : IComparable<Animation>
    {
        public AnimationType Type { get; set; } = AnimationType.None;
        public List<Texture2D> FrameList { get; set; }
        public int CurrentFrame { get; set; } = 0;
        public float FrameSpeed { get; set; } = 0.1f; // In seconds per frame
        public float FrameCounter { get; set; } = 0.0f;

        public Animation(params Texture2D[] textureNames)
        {
            FrameList = new List<Texture2D>();

            foreach (Texture2D t in textureNames)
            {
                FrameList.Add(t);
            }
        }

        public Animation(AnimationType aType, params Texture2D[] textureNames) : this(textureNames)
        {
            Type = aType;
        }

        public int CompareTo(Animation other)
        {
            return this.Type.CompareTo(other.Type);
        }
    }
}
