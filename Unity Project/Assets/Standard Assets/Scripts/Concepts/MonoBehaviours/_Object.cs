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
			_Data.componentsDatas = new _Component.Data[components.Length];
			for (int i = 0; i < components.Length; i ++)
			{
				_Component component = components[i];
				_Component.Data componentData = null;
				if (i == 0)
				{
					ObjectData obData = (ObjectData) component;
					componentData = obData._Data;
					componentData.Set (obData);
				}
				else
				{
					_Transform trs = component as _Transform;
					if (trs != null)
					{
						componentData = trs._Data;
						componentData.Set (trs);
					}
					else
					{
						_Image img = component as _Image;
						if (img != null)
						{
							componentData = img._Data;
							componentData.Set (img);
						}
						else
						{
							
						}
					}
				}
				_Data.componentsDatas[i] = component._Data;
			}
		}

		void SetComponentsIdsFromData ()
		{
			for (int i = 0; i < _Data.componentsDatas.Length; i ++)
			{
				_Component.Data componentData =_Data.componentsDatas[i];
				_Component component = obData;
				if (i > 0)
					component = EternityEngine.instance.AddComponent (this, componentData.templatePrefabIdx);
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
				_Object ob = Instantiate(EternityEngine.instance.obPrefab);
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
				ob.Init ();
				EternityEngine.obs = EternityEngine.obs.Add(ob);
				for (int i = 0; i < ob.components.Length; i ++)
				{
					_Component component = ob.components[i];
					ob.SetupComponent (component, i);
					InspectorPanel.AddOrUpdateEntries (component);
				}
				ob.SetSelectedFromData ();
			}
		}
	}
}