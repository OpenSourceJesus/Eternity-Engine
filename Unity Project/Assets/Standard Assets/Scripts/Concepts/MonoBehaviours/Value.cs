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
		[HideInInspector]
		public ValueSetter[] setters = new ValueSetter[0];

		public virtual void Awake ()
		{
			if (component != null)
				onChanged += HandleChange;
		}

		public virtual void OnDestroy ()
		{
			if (component != null)
				onChanged -= HandleChange;
		}

		void HandleChange ()
		{
			if (!component.inspectorEntries[0].gameObject.activeSelf)
				for (int i = 0; i < setters.Length; i ++)
				{
					ValueSetter setableValue = setters[i];
					setableValue.setter.text = "" + val;
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
			SetSettersIdsOfData ();
			SetComponentIdOfData ();
		}

		void SetSettersIdsOfData ()
		{
			_Data.settersIds = new string[setters.Length];
			for (int i = 0; i < setters.Length; i ++)
			{
				ValueSetter setter = setters[i];
				_Data.settersIds[i] = setter.id;
			}
		}

		void SetSettersIdsFromData ()
		{
			setters = new ValueSetter[_Data.settersIds.Length];
			for (int i = 0; i < _Data.settersIds.Length; i ++)
			{
				string setterId =_Data.settersIds[i];
				ValueSetter setter = Get<ValueSetter>(setterId);
				if (setter == null)
					setter = (ValueSetter) SaveAndLoadManager.saveData.assetsDatasDict[setterId].GenAsset();
				setters[i] = setter;
			}
		}

		void SetComponentIdOfData ()
		{
			_Data.componentId = component.id;
		}

		void SetComponentIdFromData ()
		{
			_Component component = Get<_Component>(_Data.componentId);
			if (component == null)
				component = (_Component) SaveAndLoadManager.saveData.assetsDatasDict[_Data.componentId].GenAsset();
		}

		[Serializable]
		public class Data : Asset.Data
		{
			public string[] settersIds = new string[0];
			public string componentId;

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				Value<T> value = (Value<T>) asset;
				value.SetSettersIdsFromData ();
				value.SetComponentIdFromData ();
			}
		}
	}
}