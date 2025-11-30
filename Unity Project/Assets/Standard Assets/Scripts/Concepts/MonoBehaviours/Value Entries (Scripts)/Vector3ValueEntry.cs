using UnityEngine;

namespace EternityEngine
{
	public class Vector3ValueEntry : ValueEntry<Vector3>
	{
		public FloatValueEntry xValueEntry;
		public FloatValueEntry yValueEntry;
		public FloatValueEntry zValueEntry;

		public override Vector3 GetValue ()
		{
			return new Vector3(xValueEntry.GetValue(), yValueEntry.GetValue(), zValueEntry.GetValue());
		}

		public override void UpdateDisplay (Vector3 val)
		{
			xValueEntry.UpdateDisplay(val.x);
			yValueEntry.UpdateDisplay(val.y);
			zValueEntry.UpdateDisplay(val.z);
		}
	}
}