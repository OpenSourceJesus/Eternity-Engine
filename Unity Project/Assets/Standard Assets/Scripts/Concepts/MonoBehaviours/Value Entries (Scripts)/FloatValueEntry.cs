namespace EternityEngine
{
	public class FloatValueEntry : ValueEntry<float>
	{
		public override void TrySet (string text)
		{
			float prevVal = value.val;
			if (float.TryParse(text, out value.val) && value.val != prevVal)
				value._OnChanged ();
		}
	}
}