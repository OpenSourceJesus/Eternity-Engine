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
			if (_Data == null)
				_Data = new Data();
		}

		public override void SetData ()
		{
			base.SetData ();
			SetComponentsNamesOFData ();
		}

		void SetComponentsNamesOFData ()
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
				string componentName = _Data.componentsNames[i];
				bool foundComponent = false;
				for (int i2 = 0; i2 < EternityEngine.components.Length; i2 ++)
				{
					_Component component = EternityEngine.components[i2];
					if (component.name == componentName)
					{
						components[i] = component;
						foundComponent = true;
						break;
					}
				}
				if (!foundComponent)
					components[i] = Get<_Component>(componentName);
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
				base.Apply (asset);
				_Object ob = (_Object) asset;
				ob._Data = this;
				ob.SetComponentsNamesFromData ();
			}
		}
	}
}