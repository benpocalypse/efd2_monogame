using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;
using Microsoft.Xna.Framework.Content;

namespace EfD2.Systems.SupportSystems
{
	public class TimeSystem
	{
        public Filter filterMatch
        {
            get { return new Filter().AllOf(typeof(Ephemeral)); }
        }

        public TimeSystem()
		{
		}

		public void Update(GameTime gameTime)
		{
			var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
			{
				var ephemeral = e.GetComponent<Ephemeral>();

                if (ephemeral.Active == true)
                {
                    if (ephemeral.Time < ephemeral.PersistTime)
                        ephemeral.Time += delta;

                    if (ephemeral.Time >= ephemeral.PersistTime)
                    {
                        if ((ephemeral.Repetitions > 0) && (ephemeral.RepetitionCount < ephemeral.Repetitions))
                        {
                            ephemeral.Time = 0;
                            ephemeral.RepetitionCount++;
                        }
                        else
                        {
                            // FIXME - May not want to remove the Entity here. Although...maybe we do?
                            EntityMatcher.Remove(e);
                        }
                    }
                }
			}
		}
	}
}
