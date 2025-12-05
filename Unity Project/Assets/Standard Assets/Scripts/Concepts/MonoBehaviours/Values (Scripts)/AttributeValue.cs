using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace EternityEngine
{
	public class AttributeValue : Value<Dictionary<string, string>>
	{
		public virtual void Set (Dictionary<string, string> val)
		{
			Dictionary<string, string> prevVal = this.val;
			this.val = val;
			for (int i = 0; i < entries.Length; i ++)
			{
				ValueEntry<Dictionary<string, string>> entry = entries[i];
				entry.UpdateDisplay (val);
			}
			if (!prevVal.Equals(val))
				_OnChanged ();
		}
	}
}