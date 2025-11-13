using TMPro;

namespace EternityEngine
{
	public class StringValueEntry : ValueEntry<string>
	{
		public override void TrySet (string text)
		{
			string prevVal = value.val;
			value.val = text;
			if (value.val != prevVal)
				value._OnChanged ();
		}
	}
}