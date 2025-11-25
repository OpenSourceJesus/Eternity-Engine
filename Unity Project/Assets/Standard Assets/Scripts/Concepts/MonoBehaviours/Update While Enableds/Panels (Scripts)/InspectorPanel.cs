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
			if (selectedHierarchyEntries.Length == 0)
				return;
			if (selectedHierarchyEntries.Length == 1)
			{
				HierarchyEntry hierarchyEntry = selectedHierarchyEntries[0];
				_Component[] components = hierarchyEntry.ob.components;
				for (int i = 0; i < components.Length; i ++)
				{
					_Component component = components[i];
					AddOrUpdateEntries (component);
				}
				return;
			}
			Dictionary<InspectorEntry, int> componentTypeCounts = new Dictionary<InspectorEntry, int>();
			for (int i = 0; i < selectedHierarchyEntries.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selectedHierarchyEntries[i];
				_Component[] components = hierarchyEntry.ob.components;
				HashSet<InspectorEntry> seenTypes = new HashSet<InspectorEntry>();
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					InspectorEntry entryPrefab = component.inspectorEntryPrefab;
					if (!seenTypes.Contains(entryPrefab))
					{
						seenTypes.Add(entryPrefab);
						if (componentTypeCounts.ContainsKey(entryPrefab))
							componentTypeCounts[entryPrefab] ++;
						else
							componentTypeCounts[entryPrefab] = 1;
					}
				}
			}
			HierarchyEntry firstHierarchyEntry = selectedHierarchyEntries[0];
			_Component[] firstComponents = firstHierarchyEntry.ob.components;
			for (int i = 0; i < firstComponents.Length; i ++)
			{
				_Component component = firstComponents[i];
				InspectorEntry entryPrefab = component.inspectorEntryPrefab;
				if (componentTypeCounts.TryGetValue(entryPrefab, out int cnt) && cnt == selectedHierarchyEntries.Length)
					AddOrUpdateEntries (component);
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
					{
						for (int i3 = 0; i3 < entry.boolValuesEntries.Length; i3 ++)
						{
							BoolValueEntry boolValuesEntry = entry.boolValuesEntries[i3];
							boolValuesEntry.DetachValues ();
						}
						for (int i3 = 0; i3 < entry.floatValuesEntries.Length; i3 ++)
						{
							FloatValueEntry floatValueEntry = entry.floatValuesEntries[i3];
							floatValueEntry.DetachValues ();
						}
						for (int i3 = 0; i3 < entry.stringValuesEntries.Length; i3 ++)
						{
							StringValueEntry stringValueEntry = entry.stringValuesEntries[i3];
							stringValueEntry.DetachValues ();
						}
						for (int i3 = 0; i3 < entry.vector2ValuesEntries.Length; i3 ++)
						{
							Vector2ValueEntry vector2ValueEntry = entry.vector2ValuesEntries[i3];
							vector2ValueEntry.DetachValues ();
						}
						for (int i3 = 0; i3 < entry.vector3ValuesEntries.Length; i3 ++)
						{
							Vector3ValueEntry vector3ValueEntry = entry.vector3ValuesEntries[i3];
							vector3ValueEntry.DetachValues ();
						}
						for (int i3 = 0; i3 < entry.colorValueEntries.Length; i3 ++)
						{
							ColorValueEntry colorValueEntry = entry.colorValueEntries[i3];
							colorValueEntry.DetachValues ();
						}
						Destroy(entry.gameObject);
						_Component component = entry.component;
						if (component != null)
							component.inspectorEntries = component.inspectorEntries.Remove(entry);
					}
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
				InspectorEntry entryPrefab = entry.component.inspectorEntryPrefab;
				if (entryPrefab == component.inspectorEntryPrefab)
					return new InspectorEntry[0];
			}
			InspectorEntry[] output = new InspectorEntry[instances.Length];
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				InspectorEntry entry = null;
				HierarchyEntry[] selectedHierarchyEntries = HierarchyPanel.instances[0].selected;
				if (selectedHierarchyEntries.Length < 2)
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
					InspectorEntry entryPrefab = component.inspectorEntryPrefab;
					if (!entreisForEntriesPrefabsDict.TryGetValue(entryPrefab, out InspectorEntry[] entriesForEntriesPrefabs) || entriesForEntriesPrefabs == null || entriesForEntriesPrefabs.Length == 0)
					{
						entry = inspectorPanel.NewEntry(component);
						inspectorPanel.entries = inspectorPanel.entries.Add(entry);
						output[i] = entry;
						continue;
					}
					List<_Component> selectedComponents = new List<_Component>();
					for (int i2 = 0; i2 < selectedHierarchyEntries.Length; i2 ++)
					{
						HierarchyEntry hierarchyEntry = selectedHierarchyEntries[i2];
						_Component[] obComponents = hierarchyEntry.ob.components;
						for (int i3 = 0; i3 < obComponents.Length; i3 ++)
						{
							_Component obComponent = obComponents[i3];
							if (obComponent.inspectorEntryPrefab == entryPrefab)
								selectedComponents.Add(obComponent);
						}
					}
					if (selectedComponents.Count == 0)
					{
						entry = inspectorPanel.NewEntry(component);
						inspectorPanel.entries = inspectorPanel.entries.Add(entry);
						output[i] = entry;
						continue;
					}
					float?[] floats = new float?[entryPrefab.floatValuesEntries.Length];
					string[] strings = new string[entryPrefab.stringValuesEntries.Length];
					Color?[] colors = new Color?[entryPrefab.colorValueEntries.Length];
					for (int i2 = 0; i2 < selectedComponents.Count; i2 ++)
					{
						_Component _component = selectedComponents[i2];
						for (int i3 = 0; i3 < floats.Length; i3 ++)
						{
							FloatValue floatValue = _component.floatValues[i3];
							float f = floatValue.val;
							if (i2 == 0)
								floats[i3] = f;
							else
							{
								float? _f = floats[i3];
								if (!_f.HasValue || f != _f.Value)
									floats[i3] = null;
							}
						}
						for (int i3 = 0; i3 < strings.Length; i3 ++)
						{
							StringValue stringValue = _component.stringValues[i3];
							string str = stringValue.val;
							if (i2 == 0)
								strings[i3] = str;
							else
							{
								string _str = strings[i3];
								if (_str == null || str != _str)
									strings[i3] = null;
							}
						}
						for (int i3 = 0; i3 < colors.Length; i3 ++)
						{
							ColorValue colorValue = _component.colorValues[i3];
							Color clr = colorValue.val;
							if (i2 == 0)
								colors[i3] = clr;
							else
							{
								Color? _clr = colors[i3];
								if (!_clr.HasValue || clr != _clr.Value)
									colors[i3] = null;
							}
						}
					}
					_Component[] components = selectedComponents.ToArray();
					entry = inspectorPanel.NewEntry(components);
					for (int i2 = 0; i2 < floats.Length; i2 ++)
					{
						float? f = floats[i2];
						if (f == null)
							entry.floatValuesEntries[i2].inputField.text = "—";
						else
							entry.floatValuesEntries[i2].inputField.text = "" + f;
					}
					for (int i2 = 0; i2 < strings.Length; i2 ++)
					{
						string str = strings[i2];
						if (str == null)
							entry.stringValuesEntries[i2].inputField.text = "—";
						else
							entry.stringValuesEntries[i2].inputField.text = "" + str;
					}
					for (int i2 = 0; i2 < colors.Length; i2 ++)
					{
						Color? clr = colors[i2];
						entry.colorValueEntries[i2].multipleValuesIndctrGo.gameObject.SetActive(clr == null);
					}
				}
				inspectorPanel.entries = inspectorPanel.entries.Add(entry);
				output[i] = entry;
			}
			return output;
		}

		InspectorEntry NewEntry (params _Component[] components)
		{
			_Component component = components[0];
			InspectorEntry entry = Instantiate(component.inspectorEntryPrefab, entriesParent);
			entry.inspectorPanel = this;
			entry.SetValueEntries (components);
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