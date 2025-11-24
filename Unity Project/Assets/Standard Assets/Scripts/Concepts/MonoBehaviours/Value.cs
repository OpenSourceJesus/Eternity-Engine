using System;
using UnityEngine;

namespace EternityEngine
{
	public class Value<T> : Asset
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
		public T val;
		public delegate void OnChanged();
		public event OnChanged onChanged = () => {};
		public _Component component;
		public ValueEntry<T>[] entries = new ValueEntry<T>[0];

		public override void Awake ()
		{
			base.Awake ();
			if (component != null)
				onChanged += HandleChange;
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			if (component != null)
				onChanged -= HandleChange;
		}

		void HandleChange ()
		{
			if (!component.inspectorEntries[0].gameObject.activeSelf)
				for (int i = 0; i < entries.Length; i ++)
				{
					ValueEntry<T> entry = entries[i];
					entry.UpdateDisplay (val);
				}
		}

		public void _OnChanged ()
		{
			onChanged ();
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
			SetEntriesIdsOfData ();
			SetComponentIdOfData ();
		}

		void SetEntriesIdsOfData ()
		{
			_Data.entriesIds = new string[entries.Length];
			for (int i = 0; i < entries.Length; i ++)
			{
				ValueEntry<T> entry = entries[i];
				_Data.entriesIds[i] = entry.id;
			}
		}

		void SetEntriesIdsFromData ()
		{
			entries = new ValueEntry<T>[_Data.entriesIds.Length];
			for (int i = 0; i < entries.Length; i ++)
			{
				string entryId =_Data.entriesIds[i];
				ValueEntry<T> entry = Get<ValueEntry<T>>(entryId);
				if (entry == null)
					entry = (ValueEntry<T>) SaveAndLoadManager.saveData.assetsDatasDict[entryId].GenAsset();
				entries[i] = entry;
			}
		}

		void SetComponentIdOfData ()
		{
			if (component != null)
				_Data.componentId = component.id;
		}

		void SetComponentIdFromData ()
		{
			if (_Data.componentId != null)
			{
				_Component component = Get<_Component>(_Data.componentId);
				if (component == null)
					component = (_Component) SaveAndLoadManager.saveData.assetsDatasDict[_Data.componentId].GenAsset();
			}
		}

		public virtual void Set (T val)
		{
			this.val = val;
			for (int i = 0; i < entries.Length; i ++)
			{
				ValueEntry<T> entry = entries[i];
				entry.UpdateDisplay (val);
			}
		}

		[Serializable]
		public class Data : Asset.Data
		{
			public string[] entriesIds = new string[0];
			public string componentId;

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				Value<T> value = (Value<T>) asset;
				value.SetEntriesIdsFromData ();
				value.SetComponentIdFromData ();
			}
		}
	}
}