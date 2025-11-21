using System;

namespace EternityEngine
{
	public class ObjectData : _Component
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
		public BoolValue export;

		public override void InitData ()
		{
			if (data == null)
				data = new Data();
		}

		public override void SetData ()
		{
			InitData ();
			base.SetData ();
			SetExportOfData ();
		}

		void SetExportOfData ()
		{
			_Data.export = export.val;
		}

		void SetExportFromData ()
		{
			export.val = _Data.export;
		}

		[Serializable]
		public class Data : _Component.Data
		{
			public bool export;

			public override object GenAsset ()
			{
				ObjectData obData = Instantiate(EternityEngine.instance.obDataPrefab);
				Apply (obData);
				return obData;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[name];
				base.Apply (asset);
				ObjectData obData = (ObjectData) asset;
				obData.SetExportFromData ();
			}
		}
	}
}