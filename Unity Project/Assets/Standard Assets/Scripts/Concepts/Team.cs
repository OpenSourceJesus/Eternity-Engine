using System;

namespace Frogger
{
	[Serializable]
	public class Team<T>
	{
		public string name;
		public T[] representatives = new T[0];

		public Team ()
		{
		}

		public Team (string name, params T[] representatives)
		{
			this.name = name;
			this.representatives = representatives;
		}
	}
}
