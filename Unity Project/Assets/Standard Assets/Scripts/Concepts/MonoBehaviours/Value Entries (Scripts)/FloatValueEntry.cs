using TMPro;

namespace EternityEngine
{
	public class FloatValueEntry : ValueEntry<float>
	{
		public TMP_InputField valueSetter;

		public override void TrySet (string text)
		{
			float prevVal = value.val;
			if (float.TryParse(text, out value.val) && value.val != prevVal)
				value._OnChanged ();
		}
	}
}