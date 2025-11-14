using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace EternityEngine
{
	public class InspectorPanel : Panel
	{
		public RectTransform entriesParent;
		[HideInInspector]
		public InspectorEntry[] entries = new InspectorEntry[0];
		[HideInInspector]
		public Image insertionIndicator;
		public RectTransform addComponentOptionsRectTrs;
		public RectTransform addComponentButtonRectTrs;
		public static bool isDraggingEntry;
		public new static InspectorPanel[] instances = new InspectorPanel[0];
		public static Dictionary<InspectorEntry, InspectorEntry[]> entreisForEntriesPrefabsDict = new Dictionary<InspectorEntry, InspectorEntry[]>();
		AddComponentOptionsUpdater addComponentOptionsUpdater;

		public override void Awake ()
		{
			base.Awake ();
			instances = instances.Add(this);
		}
		
		public override void OnDestroy ()
		{
			base.OnDestroy ();
			instances = instances.Remove(this);
		}

		public static void RegenEntries (bool destroy)
		{
			ClearEntries (destroy);
			HierarchyEntry[] selectedHierarchyEntries = HierarchyPanel.instances[0].selected;
			for (int i = 0; i < selectedHierarchyEntries.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selectedHierarchyEntries[i];
				_Component[] components = hierarchyEntry.ob.components;
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					AddOrUpdateEntries (component);
				}
			}
		}

		public static void ClearEntries (bool destroy)
		{
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				for (int i2 = 0; i2 < inspectorPanel.entries.Length; i2 ++)
				{
					InspectorEntry entry = inspectorPanel.entries[i2];
					if (destroy)
						Destroy(entry.gameObject);
					else
						entry.gameObject.SetActive(false);
				}
				inspectorPanel.entries = new InspectorEntry[0];
			}
		}

		public static InspectorEntry[] AddOrUpdateEntries (_Component component)
		{
			InspectorPanel firstInspectorPanel = instances[0];
			for (int i = 0; i < firstInspectorPanel.entries.Length; i ++)
			{
				InspectorEntry entry = firstInspectorPanel.entries[i];
				if (entry.component.inspectorEntryPrefab == component.inspectorEntryPrefab)
					return new InspectorEntry[0];
			}
			InspectorEntry[] output = new InspectorEntry[instances.Length];
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				InspectorEntry entry = null;
				HierarchyEntry[] selectedHierarchyEntries = HierarchyPanel.instances[0].selected;
				if (selectedHierarchyEntries.Length == 1)
				{
					if (component.inspectorEntries.Length <= i)
					{
						entry = inspectorPanel.NewEntry(component);
						component.inspectorEntries = component.inspectorEntries.Add(entry);
						InspectorEntry[] entriesForEntriesPrefabs = null;
						if (entreisForEntriesPrefabsDict.TryGetValue(component.inspectorEntryPrefab, out entriesForEntriesPrefabs))
							entreisForEntriesPrefabsDict[component.inspectorEntryPrefab] = entriesForEntriesPrefabs.Add(entry);
						else
							entreisForEntriesPrefabsDict[component.inspectorEntryPrefab] = new InspectorEntry[] { entry };
					}
					else
					{
						entry = component.inspectorEntries[i];
						entry.gameObject.SetActive(true);
					}
				}
				else
				{
					if (component.inspectorEntries.Length > i)
					{
						entry = component.inspectorEntries[i];
						entry.gameObject.SetActive(false);
					}
					entry = inspectorPanel.NewEntry(component);
					InspectorEntry[] entriesForEntriesPrefabs = entreisForEntriesPrefabsDict[component.inspectorEntryPrefab];
					float?[] floats = new float?[entry.floatValuesEntries.Length];
					for (int i2 = 0; i2 < entriesForEntriesPrefabs.Length; i2 ++)
					{
						InspectorEntry _entry = entriesForEntriesPrefabs[i2];
						for (int i3 = 0; i3 < _entry.floatValuesEntries.Length; i3 ++)
						{
							FloatValueEntry floatValueEntry = _entry.floatValuesEntries[i3];
							float f = floatValueEntry.value.val;
							if (i2 == 0)
								floats[i3] = f;
							else if (f != floats[i3])
							{
								floats[i3] = null;
								break;
							}
						}
					}
					for (int i2 = 0; i2 < floats.Length; i2 ++)
					{
						float? f = floats[i2];
						if (f == null)
							entry.floatValuesEntries[i2].valueSetter.text = "—";
						else
							entry.floatValuesEntries[i2].valueSetter.text = "" + f;
					}
				}
				inspectorPanel.entries = inspectorPanel.entries.Add(entry);
				output[i] = entry;
			}
			return output;
		}

		InspectorEntry NewEntry (_Component component)
		{
			InspectorEntry entry = Instantiate(component.inspectorEntryPrefab, entriesParent);
			entry.component = component;
			entry.inspectorPanel = this;
			entry.SetValueEntries (component);
			if (component.collapsed)
				entry.SetCollapsed (true);
			return entry;
		}

		public void ToggleAddComponentOptions ()
		{
			addComponentOptionsRectTrs.gameObject.SetActive(!addComponentOptionsRectTrs.gameObject.activeSelf);
			if (addComponentOptionsRectTrs.gameObject.activeSelf)
			{
				addComponentOptionsUpdater = new AddComponentOptionsUpdater(this);
				GameManager.updatables = GameManager.updatables.Add(addComponentOptionsUpdater);
			}
			else
				GameManager.updatables = GameManager.updatables.Remove(addComponentOptionsUpdater);
		}

		class AddComponentOptionsUpdater : IUpdatable
		{
			public InspectorPanel inspectorPanel;

			public AddComponentOptionsUpdater (InspectorPanel inspectorPanel)
			{
				this.inspectorPanel = inspectorPanel;
			}

			public void DoUpdate ()
			{
				if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
				{
					Vector2 mousePos = Mouse.current.position.ReadValue();
					Rect addComponentOptionsWorldRect = inspectorPanel.addComponentOptionsRectTrs.GetWorldRect();
					if (!addComponentOptionsWorldRect.Contains(mousePos))
					{
						Rect addComponentButtonWorldRect = inspectorPanel.addComponentButtonRectTrs.GetWorldRect();
						if (!addComponentButtonWorldRect.Contains(mousePos))
							inspectorPanel.addComponentOptionsRectTrs.gameObject.SetActive(false);
						GameManager.updatables = GameManager.updatables.Remove(this);
					}
				}
			}
		}
	}
}