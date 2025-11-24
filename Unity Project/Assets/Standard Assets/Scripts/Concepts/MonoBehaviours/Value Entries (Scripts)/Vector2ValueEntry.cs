using System;
using UnityEngine;

namespace EternityEngine
{
	public class Vector2ValueEntry : ValueEntry<Vector2>
	{
		public new Data _Data
		{
			get
			{
				return (Data) data;
			}
			set
			{
				data = value;
			}
		}
		public FloatValueEntry xValueEntry;
		public FloatValueEntry yValueEntry;

		public override void InitData ()
		{
			if (data == null)
				data = new Data();
		}

		[Serializable]
		public class Data : Asset.Data
		{
			public override object GenAsset ()
			{
				Vector2ValueEntry vector2ValueEntry = Instantiate(EternityEngine.instance.vector2ValueEntryPrefab);
				Apply (vector2ValueEntry);
				return vector2ValueEntry;
			}
		}
	}
}