using TMPro;

namespace EternityEngine
{
	public class BoolValueEntry : ValueEntry<bool>
	{
		public new void TrySet (bool val)
		{
			Value<bool>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<bool> target = targets[i];
				if (target == null)
					continue;
				bool prevVal = target.val;
				target.val = val;
				if (target.val != prevVal)
					target._OnChanged ();
			}
		}
	}
}