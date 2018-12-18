using ECS;

namespace EfD2
{
	internal class Ephemeral : IComponent
	{
		Entity IComponent.entity { get; set; }

		public double PersistTime = 0.0f;
		public double Time = 0.0f;
		public int Repetitions = 0;
		public int RepetitionCount = 0;
        public bool Active = true;

		public Ephemeral()
		{
		}
	}
}
