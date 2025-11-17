using TMPro;

namespace EternityEngine
{
	public class StringValueEntry : ValueEntry<string>
	{
		public override void TrySet (string text)
		{
			Value<string>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<string> target = targets[i];
				if (target == null)
					continue;
				string prevVal = target.val;
				target.val = text;
				if (target.val != prevVal)
					target._OnChanged ();
			}
		}
	}
}