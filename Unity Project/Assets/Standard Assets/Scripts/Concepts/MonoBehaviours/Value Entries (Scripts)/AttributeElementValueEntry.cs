using TMPro;
using System.Collections.Generic;

namespace EternityEngine
{
	public class AttributeElementValueEntry<T> : ValueEntry<KeyValuePair<string, T>>
	{
		public ValueEntry<T> valueEntry;
		KeyValuePair<string, T> keyValuePair = new KeyValuePair<string, T>();

		public void SetKey (string key)
		{
			keyValuePair = new KeyValuePair<string, T>(key, default(T));
		}

		public void SetValue (T value)
		{
			keyValuePair = new KeyValuePair<string, T>(keyValuePair.Key, value);
			Value<KeyValuePair<string, T>>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<KeyValuePair<string, T>> target = targets[i];
				if (target == null)
					continue;
				KeyValuePair<string, T> prevVal = target.val;
				target.val = keyValuePair;
				if (!target.val.Equals(prevVal))
					target._OnChanged ();
			}
		}

		public override void UpdateDisplay (KeyValuePair<string, T> val)
		{
			inputField.text = val.Key;
			valueEntry.UpdateDisplay (val.Value);
		}

		public override KeyValuePair<string, T> GetValue ()
		{
			return new KeyValuePair<string, T>(inputField.text, valueEntry.GetValue());
		}
	}
}