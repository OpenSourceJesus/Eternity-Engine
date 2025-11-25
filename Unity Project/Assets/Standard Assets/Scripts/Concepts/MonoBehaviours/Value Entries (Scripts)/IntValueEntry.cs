using TMPro;

namespace EternityEngine
{
	public class IntValueEntry : ValueEntry<int>
	{
		public override void TrySet (string text)
		{
			Value<int>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			bool parsed = false;
			int newVal = 0;
			if (string.IsNullOrEmpty(text))
			{
				parsed = true;
				newVal = 0;
			}
			else
				parsed = int.TryParse(text, out newVal);
			if (!parsed)
				return;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<int> target = targets[i];
				if (target == null || target.val == newVal)
					continue;
				target.val = newVal;
				target._OnChanged ();
			}
		}
	}
}