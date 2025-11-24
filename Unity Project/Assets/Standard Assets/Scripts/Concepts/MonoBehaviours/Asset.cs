using System;
using UnityEngine;

namespace EternityEngine
{
	public class Asset : MonoBehaviour
	{
		[HideInInspector]
		public string id;
		public object data;
		public bool save;
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
		static string lastId = "" + (char) 0;

		public virtual void Awake ()
		{
			if (save)
				GameManager.assets.Add(this);
			if (!SaveAndLoadManager.isLoading)
			{
				byte charVal = (byte) lastId[lastId.Length - 1];
				if (charVal == 255)
					lastId += (char) 0;
				else
				{
					charVal ++;
					lastId = lastId.Substring(0, lastId.Length - 1) + (char) charVal;
				}
				id = lastId;
			}
		}

		public virtual void OnDestroy ()
		{
			if (save)
				GameManager.assets.Remove(this);
		}

		public static T Get<T> (string id) where T : Asset
		{
			for (int i = 0; i < GameManager.assets.Count; i ++)
			{
				Asset asset = GameManager.assets[i];
				if (asset.id == id)
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
			SetIdOfData ();
		}

		public void SetIdOfData ()
		{
			_Data.id = id;
		}

		public void SetIdFromData ()
		{
			id = _Data.id;
			if (id.Length > lastId.Length)
				lastId = id;
			else if (id.Length == lastId.Length && (byte) id[id.Length - 1] > (byte) lastId[lastId.Length - 1])
				lastId = id;
		}

		[Serializable]
		public class Data
		{
			public string id;

			public virtual object GenAsset ()
			{
				throw new NotImplementedException();
			}

			public virtual void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				asset.SetIdFromData ();
			}
		}
	}
}