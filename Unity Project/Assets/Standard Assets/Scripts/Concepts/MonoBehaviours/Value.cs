using UnityEngine;

namespace EternityEngine
{
	public class Value<T> : MonoBehaviour
	{
		public T val;
		public delegate void OnChanged();
		public event OnChanged onChanged;

		public void _OnChanged ()
		{
			onChanged ();
		}
	}
}