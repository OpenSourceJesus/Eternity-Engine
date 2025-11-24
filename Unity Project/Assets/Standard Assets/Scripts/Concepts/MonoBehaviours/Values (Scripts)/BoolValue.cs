using System;

namespace EternityEngine
{
	public class BoolValue : Value<bool>
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

		public override void InitData ()
		{
			if (data == null)
				data = new Data();
		}

		public override void SetData ()
		{
			InitData ();
			base.SetData ();
			SetValOfData ();
		}

		void SetValOfData ()
		{
			_Data.val = val;
		}

		void SetValFromData ()
		{
			Set (_Data.val);
		}

		[Serializable]
		public class Data : Value<bool>.Data
		{
			public bool val;

			public override object GenAsset ()
			{
				BoolValue boolValue = Instantiate(EternityEngine.instance.boolValuePrefab);
				Apply (boolValue);
				return boolValue;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				BoolValue boolValue = (BoolValue) asset;
				boolValue.SetValFromData ();
			}
		}
	}
}