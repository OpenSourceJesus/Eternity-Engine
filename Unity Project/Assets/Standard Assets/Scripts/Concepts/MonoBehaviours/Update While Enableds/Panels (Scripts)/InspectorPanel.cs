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
		static Dictionary<InspectorEntry, InspectorEntry[]> entreisForEntriesPrefabsDict = new Dictionary<InspectorEntry, InspectorEntry[]>();

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

		public static void RegenEntries ()
		{
			ClearEntries ();
			HierarchyEntry[] selected = HierarchyPanel.instances[0].selected;
			for (int i = 0; i < selected.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selected[i];
				_Component[] components = hierarchyEntry.ob.components;
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					AddOrUpdateEntries (component);
				}
			}
		}

		public static void ClearEntries ()
		{
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				for (int i2 = 0; i2 < inspectorPanel.entriesParent.childCount; i2 ++)
					inspectorPanel.entriesParent.GetChild(i2).gameObject.SetActive(false);
				inspectorPanel.entries = new InspectorEntry[0];
			}
		}

		public static InspectorEntry[] AddOrUpdateEntries (_Component component)
		{
			InspectorEntry[] output = new InspectorEntry[instances.Length];
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				InspectorEntry entry = null;
				InspectorEntry[] entriesPrefabs = new InspectorEntry[entreisForEntriesPrefabsDict.Count];
				entreisForEntriesPrefabsDict.Keys.CopyTo(entriesPrefabs, 0);
				int entryPrefabIdx = entriesPrefabs.IndexOf(component.inspectorEntryPrefab);
				if (entryPrefabIdx == -1)
				{
					if (component.inspectorEntries.Length <= i)
					{
						entry = Instantiate(component.inspectorEntryPrefab, inspectorPanel.entriesParent);
						entry.component = component;
						entry.inspectorPanel = inspectorPanel;
						for (int i2 = 0; i2 < component.floatValues.Length; i2 ++)
						{
							FloatValue floatValue = component.floatValues[i2];
							entry.floatValuesEntries[i2].value = floatValue;
						}
						for (int i2 = 0; i2 < component.vector3Values.Length; i2 ++)
						{
							Vector3Value vector3Value = component.vector3Values[i2];
							entry.vector3ValuesEntries[i2].value = vector3Value;
						}
						if (component.collapsed)
							entry.SetCollapsed (true);
						component.inspectorEntries = component.inspectorEntries.Add(entry);
						if (i == 0)
							entreisForEntriesPrefabsDict[component.inspectorEntryPrefab] = new InspectorEntry[] { entry };
						else
							entreisForEntriesPrefabsDict[component.inspectorEntryPrefab] = entreisForEntriesPrefabsDict[component.inspectorEntryPrefab].Add(entry);
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
					entry = Instantiate(component.inspectorEntryPrefab, inspectorPanel.entriesParent);
					entry.component = component;
					entry.inspectorPanel = inspectorPanel;
					for (int i2 = 0; i2 < component.floatValues.Length; i2 ++)
					{
						FloatValue floatValue = component.floatValues[i2];
						entry.floatValuesEntries[i2].value = floatValue;
					}
					for (int i2 = 0; i2 < component.vector3Values.Length; i2 ++)
					{
						Vector3Value vector3Value = component.vector3Values[i2];
						entry.vector3ValuesEntries[i2].value = vector3Value;
					}
					if (component.collapsed)
						entry.SetCollapsed (true);
					InspectorEntry[] entriesForEntriesPrefabs = entreisForEntriesPrefabsDict[component.inspectorEntryPrefab];
					float?[] floats = new float?[entriesForEntriesPrefabs.Length];
					Vector3?[] vector3s = new Vector3?[entriesForEntriesPrefabs.Length];
					for (int i2 = 0; i2 < entriesForEntriesPrefabs.Length; i2 ++)
					{
						InspectorEntry _entry = entriesForEntriesPrefabs[i2];
						for (int i3 = 0; i3 < _entry.floatValuesEntries.Length; i3 ++)
						{
							FloatValueEntry floatValueEntry = _entry.floatValuesEntries[i3];
							float f = floatValueEntry.value.val;
							if (i3 == 0)
								floats[i2] = f;
							else if (f != floats[i2])
							{
								floats[i2] = null;
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

		public void ToggleAddComponentOptions ()
		{
			addComponentOptionsRectTrs.gameObject.SetActive(!addComponentOptionsRectTrs.gameObject.activeSelf);
			GameManager.updatables = GameManager.updatables.Add(new AddComponentOptionsUpdater(this));
		}

		public class AddComponentOptionsUpdater : IUpdatable
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