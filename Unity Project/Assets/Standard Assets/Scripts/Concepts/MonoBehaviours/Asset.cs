using System;
using UnityEngine;

namespace EternityEngine
{
	public class Asset : Spawnable
	{
		public object data;
		public Data _Data
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

		public virtual void Awake ()
		{
			InitData ();
			GameManager.assetsDatas.Add(_Data);
		}

		public virtual void OnDestroy ()
		{
			GameManager.assetsDatas.Remove(_Data);
		}

		public virtual void InitData ()
		{
			if (data == null)
				data = new Data();
		}

		public virtual void SetData ()
		{
			InitData ();
			SetNameOfData ();
		}

		void SetNameOfData ()
		{
			_Data.name = name;
		}

		void SetNameFromData ()
		{
			name = _Data.name;
		}

		[Serializable]
		public class Data
		{
			public string name;

			public virtual object MakeAsset ()
			{
				throw new NotImplementedException();
			}

			public virtual void Apply (Asset asset)
			{
				if (asset.data == null)
					asset.data = this;
				asset.SetNameFromData ();
			}
		}
	}
}