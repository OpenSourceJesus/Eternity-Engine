using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace EternityEngine
{
	public class AttributeValue<T> : Value<Dictionary<string, T>>
	{
		public override void Awake ()
		{
			base.Awake ();
			val = new Dictionary<string, T>();
		}

		public override void Set (Dictionary<string, T> val)
		{
			Dictionary<string, T> prevVal = this.val;
			this.val = val;
			for (int i = 0; i < entries.Length; i ++)
			{
				ValueEntry<Dictionary<string, T>> entry = entries[i];
				entry.UpdateDisplay (val);
			}
			if (!prevVal.Equals(val))
				_OnChanged ();
		}
	}
}