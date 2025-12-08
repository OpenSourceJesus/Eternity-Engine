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
				_Component.Data data = component.GetData();
				data.Set (component);
				componentsDatas.Add(component._Data);
			}
			_Data.componentsDatas = componentsDatas.ToArray();
		}

		void SetComponentsIdsFromData ()
		{
			List<_Component> unmatchedComponents = new List<_Component>(components);
			for (int i = 0; i < _Data.componentsDatas.Length; i ++)
			{
				_Component.Data componentData = _Data.componentsDatas[i];
				_Component component = null;
				bool addNewComponent = true;
				for (int i2 = 0; i2 < unmatchedComponents.Count; i2 ++)
				{
					_Component _component = unmatchedComponents[i2];
					if (_component.prefabIdx == componentData.prefabIdx)
					{
						component = _component;
						addNewComponent = false;
						unmatchedComponents.RemoveAt(i2);
						break;
					}
				}
				if (addNewComponent)
					component = EternityEngine.instance.AddComponent(this, componentData.prefabIdx);
				componentData.Apply (component);
			}
			for (int i = 0; i < components.Length; i ++)
			{
				_Component component = components[i];
				for (int i2 = i + 1; i2 < components.Length; i2 ++)
				{
					_Component component2 = components[i2];
					if (component.requiredComponentsIdxs.Contains(component2.prefabIdx))
					{
						component.dependsOn.Add(component2);
						component2.dependsOnMe.Add(component);
						break;
					}
				}
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
				_Object ob = EternityEngine.instance.NewObject(EternityEngine.instance.obPrefab, name);
				Apply (ob);
				return ob;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				_Object ob = (_Object) asset;
				ob.SetComponentsIdsFromData ();
				ob.SetSelectedFromData ();
			}
		}
	}
}