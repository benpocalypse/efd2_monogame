using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECS;

using Microsoft.Xna.Framework.Graphics;

namespace SharpECS.Samples.Components
{
    internal class GraphicsComponent 
        : IComponent
    {
        Entity IComponent.entity { get; set; }
        public Texture2D Texture { get; set; }
    }
}
