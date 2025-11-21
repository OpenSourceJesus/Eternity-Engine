using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Object : Asset
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
		public ObjectData obData;
		public _Component[] components = new _Component[0];
		[HideInInspector]
		public SceneEntry[] sceneEntries = new SceneEntry[0];
		[HideInInspector]
		public HierarchyEntry[] hierarchyEntries = new HierarchyEntry[0];

		public override void InitData ()
		{
			if (data == null)
				data = new Data();
		}

		public override void SetData ()
		{
			InitData ();
			base.SetData ();
			SetComponentsNamesOfData ();
		}

		void SetComponentsNamesOfData ()
		{
			_Data.componentsNames = new string[components.Length];
			for (int i = 0; i < components.Length; i ++)
			{
				_Component component = components[i];
				_Data.componentsNames[i] = component.name;
			}
		}

		void SetComponentsNamesFromData ()
		{
			components = new _Component[_Data.componentsNames.Length];
			for (int i = 0; i < _Data.componentsNames.Length; i ++)
			{
				string componentName =_Data.componentsNames[i];
				_Component component = null;
				component = Get<_Component>(componentName);
				if (component == null)
					component = (_Component) SaveAndLoadManager.saveData.assetsDatasDict[componentName].GenAsset();
				components[i] = component;
			}
		}

		[Serializable]
		public class Data : Asset.Data
		{
			public string[] componentsNames = new string[0];

			public override object GenAsset ()
			{
				_Object ob = Instantiate(EternityEngine.instance.obPrefab);
				Apply (ob);
				return ob;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[name];
				base.Apply (asset);
				_Object ob = (_Object) asset;
				ob.SetComponentsNamesFromData ();
			}
		}
	}
}