using System;

namespace EternityEngine
{
	public class FloatValue : Value<float>
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
		public class Data : Value<float>.Data
		{
			public float val;

			public override object GenAsset ()
			{
				FloatValue floatValue = Instantiate(EternityEngine.instance.floatValuePrefab);
				Apply (floatValue);
				return floatValue;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				FloatValue floatValue = (FloatValue) asset;
				floatValue.SetValFromData ();
			}
		}
	}
}