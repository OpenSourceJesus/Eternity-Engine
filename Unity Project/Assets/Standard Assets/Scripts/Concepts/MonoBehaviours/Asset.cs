using System;
using UnityEngine;

namespace EternityEngine
{
	public class Asset : MonoBehaviour
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
			GameManager.assets.Add(this);
			GameManager.assetsDatas.Add(_Data);
		}

		public virtual void OnDestroy ()
		{
			GameManager.assets.Remove(this);
			GameManager.assetsDatas.Remove(_Data);
		}

		public static T Get<T> (string name) where T : Asset
		{
			for (int i = 0; i < GameManager.assetsDatas.Count; i ++)
			{
				Asset.Data data = GameManager.assetsDatas[i];
				if (data.name == name)
					return (T) data.GenAsset();
			}
			return null;
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

		public void SetNameOfData ()
		{
			_Data.name = name;
		}

		public void SetNameFromData ()
		{
			name = _Data.name;
		}

		[Serializable]
		public class Data
		{
			public string name;

			public virtual object GenAsset ()
			{
				throw new NotImplementedException();
			}

			public virtual void Apply (Asset asset)
			{
				asset.data = this;
				asset.SetNameFromData ();
			}
		}
	}
}