using ECS;

namespace EfD2
{
	internal class Ephemeral : IComponent
	{
		Entity IComponent.entity { get; set; }
		public double PersistTime = 0.0f;

		public Ephemeral()
		{
		}
	}
}
