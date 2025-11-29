using TMPro;
using UnityEngine;

namespace EternityEngine
{
	public class ArrayValue<T> : Value<T[]>
	{
		public Value<T> eltPrefab;
		public bool canResize;
        public Value<T>[] elts = new Value<T>[0];

		public virtual void Set (T[] val)
		{
			T[] prevVal = this.val;
			this.val = val;
			for (int i = 0; i < entries.Length; i ++)
			{
				ValueEntry<T[]> entry = entries[i];
				entry.UpdateDisplay (val);
			}
			if (!prevVal.Equals(val))
				_OnChanged ();
		}
	}
}