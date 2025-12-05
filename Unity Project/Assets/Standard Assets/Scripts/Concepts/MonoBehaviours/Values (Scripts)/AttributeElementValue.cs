using System.Collections.Generic;

namespace EternityEngine
{
	public class AttributeElementValue<T> : Value<KeyValuePair<string, T>>
	{
		public override void Set (KeyValuePair<string, T> val)
		{
			KeyValuePair<string, T> prevVal = this.val;
			this.val = val;
			for (int i = 0; i < entries.Length; i ++)
			{
				ValueEntry<KeyValuePair<string, T>> entry = entries[i];
				entry.UpdateDisplay (val);
			}
			if (!prevVal.Equals(val))
				_OnChanged ();
		}
	}
}