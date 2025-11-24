using System;
using TMPro;
using UnityEngine.UI;

namespace EternityEngine
{
	public class BoolValueEntry : ValueEntry<bool>
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
		public Toggle toggle;

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

		public override void UpdateDisplay (bool val)
		{
			toggle.isOn = val;
		}

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
				BoolValueEntry boolValuesEntry = Instantiate(EternityEngine.instance.boolValuesEntryPrefab);
				Apply (boolValuesEntry);
				return boolValuesEntry;
			}
		}
	}
}