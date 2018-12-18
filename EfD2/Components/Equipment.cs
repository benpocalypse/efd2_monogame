using System;
using System.Collections.Generic;
using ECS;

namespace EfD2.Components
{
    public class Equipment : IComponent
    {
        Entity IComponent.entity { get; set; }

        public Entity Item { get; set; }

        public Equipment()
        {
        }
    }
}
