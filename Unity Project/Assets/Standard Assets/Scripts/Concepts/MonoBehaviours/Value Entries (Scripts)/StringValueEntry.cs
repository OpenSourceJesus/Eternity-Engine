using TMPro;
using System;

namespace EternityEngine
{
	public class StringValueEntry : ValueEntry<string>
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
				StringValueEntry stringValueEntry = Instantiate(EternityEngine.instance.stringValueEntryPrefab);
				Apply (stringValueEntry);
				return stringValueEntry;
			}
		}
	}
}