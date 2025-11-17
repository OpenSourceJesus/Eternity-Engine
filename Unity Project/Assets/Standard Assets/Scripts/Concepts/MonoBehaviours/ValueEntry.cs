using UnityEngine;

namespace EternityEngine
{
	public class ValueEntry<T> : SetableValue
	{
		[HideInInspector]
		public Value<T> value;
	}
}