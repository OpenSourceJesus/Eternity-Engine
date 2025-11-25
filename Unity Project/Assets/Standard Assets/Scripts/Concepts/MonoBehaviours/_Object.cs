using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
		public Transform trs;
		public ObjectData obData;
		public _Component[] components = new _Component[0];
		[HideInInspector]
		public SceneEntry[] sceneEntries = new SceneEntry[0];
		[HideInInspector]
		public HierarchyEntry[] hierarchyEntries = new HierarchyEntry[0];

		public void Init ()
		{
			hierarchyEntries = new HierarchyEntry[HierarchyPanel.instances.Length];
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				HierarchyEntry hierarchyEntry = Instantiate(EternityEngine.instance.hierarchyEntryPrefab, hierarchyPanel.entriesParent);
				hierarchyEntry.nameText.text = name;
				hierarchyEntry.ob = this;
				hierarchyEntry.hierarchyPanel = hierarchyPanel;
				hierarchyEntries[i] = hierarchyEntry;
				hierarchyPanel.entries = hierarchyPanel.entries.Add(hierarchyEntry);
			}
			sceneEntries = new SceneEntry[0];
		}

		public void SetupComponent (_Component component, int idx)
		{
			component.ob = this;
			SceneEntry sceneEntry = component.sceneEntry;
			if (sceneEntry != null)
			{
				sceneEntry = Instantiate(sceneEntry, sceneEntry.scenePanel.obsParentRectTrs);
				sceneEntry.hierarchyEntries = hierarchyEntries;
				sceneEntries = sceneEntries.Add(sceneEntry);
				component.sceneEntry = sceneEntry;
			}
			for (int i = 0; i < component.inspectorEntries.Length; i ++)
			{
				InspectorEntry inspectorEntry = component.inspectorEntries[i];
				inspectorEntry.gameObject.SetActive(false);
				inspectorEntry = Instantiate(inspectorEntry, inspectorEntry.rectTrs.parent);
				component.inspectorEntries[i] = inspectorEntry;
				inspectorEntry.SetValueEntries (component);
				InspectorEntry[] inspectorEntriesForEntriesPrefabs = null;
				if (InspectorPanel.entreisForEntriesPrefabsDict.TryGetValue(component.inspectorEntryPrefab, out inspectorEntriesForEntriesPrefabs))
					InspectorPanel.entreisForEntriesPrefabsDict[component.inspectorEntryPrefab] = inspectorEntriesForEntriesPrefabs.Add(inspectorEntry);
				else
					InspectorPanel.entreisForEntriesPrefabsDict[component.inspectorEntryPrefab] = new InspectorEntry[] { inspectorEntry };
			}
			components[idx] = component;
			component.Init ();
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
			SetNameOfData ();
			SetComponentsIdsOfData ();
			SetSelectedOfData ();
		}

		public void SetNameOfData ()
		{
			_Data.name = name;
		}

		public void SetNameFromData ()
		{
			name = _Data.name;
		}

		void SetComponentsIdsOfData ()
		{
			List<_Component.Data> componentsDatas = new List<_Component.Data>(); 
			for (int i = 0; i < components.Length; i ++)
			{
				_Component component = components[i];
				(_Component.Data data, _Component component) dataAndComponent = component.GetDataAndComponent();
				dataAndComponent.data.Set (dataAndComponent.component);
				componentsDatas.Add(dataAndComponent.data);
			}
			componentsDatas.Sort(new ComponentDataComparer());
			_Data.componentsDatas = componentsDatas.ToArray();
		}

		void SetComponentsIdsFromData ()
		{
			for (int i = 0; i < _Data.componentsDatas.Length; i ++)
			{
				_Component.Data componentData = _Data.componentsDatas[i];
				_Component component = null;
				bool addNewComponent = true;
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component _component = components[i2];
					if (_component.prefabIdx == componentData.prefabIdx)
					{
						component = _component.GetDataAndComponent().component;
						addNewComponent = false;
						break;
					}
				}
				if (addNewComponent)
					component = EternityEngine.instance.AddComponent (this, componentData.prefabIdx);
				componentData.Apply (component);
			}
		}

		void SetSelectedOfData ()
		{
			_Data.selected = hierarchyEntries[0].selected;
		}

		void SetSelectedFromData ()
		{
			for (int i = 0; i < hierarchyEntries.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = hierarchyEntries[i];
				hierarchyEntry.SetSelected (_Data.selected);
			}
		}

		[Serializable]
		public class Data : Asset.Data
		{
			public string name;
			public _Component.Data[] componentsDatas = new _Component.Data[0];
			public string[] hierarchyEntriesIds = new string[0];
			public bool selected;

			public override object GenAsset ()
			{
				_Object ob = EternityEngine.instance.NewPresetObject(EternityEngine.PresetObjectType.Empty);
				Apply (ob);
				return ob;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				_Object ob = (_Object) asset;
				ob.SetNameFromData ();
				ob.SetComponentsIdsFromData ();
				for (int i = 0; i < ob.components.Length; i ++)
				{
					_Component component = ob.components[i];
					ob.SetupComponent (component, i);
					InspectorPanel.AddOrUpdateEntries (component);
				}
				ob.SetSelectedFromData ();
			}
		}

		class ComponentDataComparer : IComparer<_Component.Data>
		{
			public int Compare (_Component.Data componentData, _Component.Data componentData2)
			{
				_Component componentPrefab = EternityEngine.instance.componentsPrefabs[componentData.prefabIdx];
				_Component componentPrefab2 = EternityEngine.instance.componentsPrefabs[componentData2.prefabIdx];
				if (componentPrefab.requiredComponentsIdxs.Contains(componentData2.prefabIdx))
					return -1;
				else if (componentPrefab2.requiredComponentsIdxs.Contains(componentData.prefabIdx))
					return 1;
				else
					return 0;
			}
		}
	}
}