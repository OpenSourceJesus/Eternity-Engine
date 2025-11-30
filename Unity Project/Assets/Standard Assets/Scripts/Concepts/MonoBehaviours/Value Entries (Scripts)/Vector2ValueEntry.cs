using UnityEngine;

namespace EternityEngine
{
	public class Vector2ValueEntry : ValueEntry<Vector2>
	{
		public FloatValueEntry xValueEntry;
		public FloatValueEntry yValueEntry;

		public override Vector2 GetValue ()
		{
			return new Vector2(xValueEntry.GetValue(), yValueEntry.GetValue());
		}

		public override void UpdateDisplay (Vector2 val)
		{
			xValueEntry.UpdateDisplay(val.x);
			yValueEntry.UpdateDisplay(val.y);
		}
	}
}