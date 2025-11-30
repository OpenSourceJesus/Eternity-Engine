using TMPro;
using UnityEngine;

namespace EternityEngine
{
	public class ArrayValue<T> : Value<T[]>
	{
		public bool canResize;

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