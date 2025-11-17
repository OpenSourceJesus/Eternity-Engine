using TMPro;

namespace EternityEngine
{
	public class FloatValueEntry : ValueEntry<float>
	{
		public override void TrySet (string text)
		{
			base.TrySet (text);
			float prevVal = value.val;
			if (string.IsNullOrEmpty(text) && prevVal != 0)
			{
				value.val = 0;
				value._OnChanged ();
			}
			else if (float.TryParse(text, out value.val))
			{
				if (value.val != prevVal)
					value._OnChanged ();
			}
			else
				valueSetter.text = "" + prevVal;
		}
	}
}