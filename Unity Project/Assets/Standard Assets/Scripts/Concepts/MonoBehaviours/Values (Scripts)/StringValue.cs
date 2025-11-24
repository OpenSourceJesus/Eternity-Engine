using System;

namespace EternityEngine
{
	public class StringValue : Value<string>
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
		public class Data : Value<string>.Data
		{
			public string val;

			public override object GenAsset ()
			{
				StringValue stringValue = Instantiate(EternityEngine.instance.stringValuePrefab);
				Apply (stringValue);
				return stringValue;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				StringValue stringValue = (StringValue) asset;
				stringValue.SetValFromData ();
			}
		}
	}
}