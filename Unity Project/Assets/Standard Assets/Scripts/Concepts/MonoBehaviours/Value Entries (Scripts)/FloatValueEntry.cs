using TMPro;

namespace EternityEngine
{
	public class FloatValueEntry : ValueEntry<float>
	{
		public override void TrySet (string text)
		{
			Value<float>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			bool parsed = false;
			float newVal = 0;
			if (string.IsNullOrEmpty(text))
			{
				parsed = true;
				newVal = 0;
			}
			else
				parsed = float.TryParse(text, out newVal);
			if (!parsed)
				return;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<float> target = targets[i];
				if (target == null || target.val == newVal)
					continue;
				target.val = newVal;
				target._OnChanged ();
			}
		}

		public override float GetValue ()
		{
			float val = 0;
			float.TryParse(inputField.text, out val);
			return val;
		}
	}
}