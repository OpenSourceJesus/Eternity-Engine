using UnityEngine;

namespace EternityEngine
{
	public class ValueEntry<T> : MonoBehaviour
	{
		public Value<T> value;

		public virtual void TrySet (string text)
		{
		}
	}
}