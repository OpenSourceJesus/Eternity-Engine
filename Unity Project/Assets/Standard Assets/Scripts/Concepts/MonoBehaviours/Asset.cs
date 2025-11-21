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

		void Awake ()
		{
			GameManager.assets.Add(this);
		}

		void OnDestroy ()
		{
			GameManager.assets.Remove(this);
		}

		public static T Get<T> (string name) where T : Asset
		{
			for (int i = 0; i < GameManager.assets.Count; i ++)
			{
				Asset asset = GameManager.assets[i];
				if (asset.name == name)
					return (T) asset;
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
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[name];
				asset.SetNameFromData ();
			}
		}
	}
}