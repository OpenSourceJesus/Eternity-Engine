using System;
using UnityEngine;

namespace EternityEngine
{
	public class Vector3ValueEntry : ValueEntry<Vector3>
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
		public FloatValueEntry zValueEntry;

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
				Vector3ValueEntry vector3ValueEntry = Instantiate(EternityEngine.instance.vector3ValueEntryPrefab);
				Apply (vector3ValueEntry);
				return vector3ValueEntry;
			}
		}
	}
}