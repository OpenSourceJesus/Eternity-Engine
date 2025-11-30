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
			Dictionary<InspectorEntry, int> minComponentTypeCounts = new Dictionary<InspectorEntry, int>();
			for (int i = 0; i < selectedHierarchyEntries.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selectedHierarchyEntries[i];
				_Component[] components = hierarchyEntry.ob.components;
				Dictionary<InspectorEntry, int> objTypeCounts = new Dictionary<InspectorEntry, int>();
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					InspectorEntry entryPrefab = component.inspectorEntryPrefab;
					if (objTypeCounts.ContainsKey(entryPrefab))
						objTypeCounts[entryPrefab] ++;
					else
						objTypeCounts[entryPrefab] = 1;
				}
				if (i == 0)
					foreach (KeyValuePair<InspectorEntry, int> keyValuePair in objTypeCounts)
						minComponentTypeCounts[keyValuePair.Key] = keyValuePair.Value;
				else
				{
					List<InspectorEntry> toRemove = new List<InspectorEntry>();
					Dictionary<InspectorEntry, int> toUpdate = new Dictionary<InspectorEntry, int>();
					foreach (KeyValuePair<InspectorEntry, int> keyValuePair in minComponentTypeCounts)
						if (objTypeCounts.TryGetValue(keyValuePair.Key, out int cnt))
							toUpdate[keyValuePair.Key] = Mathf.Min(keyValuePair.Value, cnt);
						else
							toRemove.Add(keyValuePair.Key);
					foreach (KeyValuePair<InspectorEntry, int> keyValuePair in toUpdate)
						minComponentTypeCounts[keyValuePair.Key] = keyValuePair.Value;
					for (int i2 = 0; i2 < toRemove.Count; i2 ++)
					{
						InspectorEntry remove = toRemove[i2];
						minComponentTypeCounts.Remove(remove);
					}
				}
			}
			HierarchyEntry firstHierarchyEntry = selectedHierarchyEntries[0];
			_Component[] firstComponents = firstHierarchyEntry.ob.components;
			Dictionary<InspectorEntry, int> processedTypeCounts = new Dictionary<InspectorEntry, int>();
			for (int i = 0; i < firstComponents.Length; i ++)
			{
				_Component component = firstComponents[i];
				InspectorEntry entryPrefab = component.inspectorEntryPrefab;
				if (!minComponentTypeCounts.TryGetValue(entryPrefab, out int minCnt) || minCnt == 0)
					continue;
				int processedCnt = 0;
				if (processedTypeCounts.TryGetValue(entryPrefab, out processedCnt) && processedCnt >= minCnt)
					continue;
				processedTypeCounts[entryPrefab] = processedCnt + 1;
				AddOrUpdateEntriesAtIndex (component, processedCnt);
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
						for (int i3 = 0; i3 < entry.intValuesEntries.Length; i3 ++)
						{
							IntValueEntry intValueEntry = entry.intValuesEntries[i3];
							intValueEntry.DetachValues ();
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
						for (int i3 = 0; i3 < entry.boolArrayValuesEntries.Length; i3 ++)
						{
							BoolArrayValueEntry boolArrayValueEntry = entry.boolArrayValuesEntries[i3];
							boolArrayValueEntry.DetachValues ();
						}
						for (int i3 = 0; i3 < entry.intArrayValuesEntries.Length; i3 ++)
						{
							IntArrayValueEntry intArrayValueEntry = entry.intArrayValuesEntries[i3];
							intArrayValueEntry.DetachValues ();
						}
						for (int i3 = 0; i3 < entry.floatArrayValuesEntries.Length; i3 ++)
						{
							FloatArrayValueEntry floatArrayValueEntry = entry.floatArrayValuesEntries[i3];
							floatArrayValueEntry.DetachValues ();
						}
						for (int i3 = 0; i3 < entry.vector2ArrayValuesEntries.Length; i3 ++)
						{
							Vector2ArrayValueEntry vector2ArrayValueEntry = entry.vector2ArrayValuesEntries[i3];
							vector2ArrayValueEntry.DetachValues ();
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
			InspectorEntry[] output = new InspectorEntry[instances.Length];
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				InspectorEntry entry = null;
				HierarchyEntry[] selectedHierarchyEntries = HierarchyPanel.instances[0].selected;
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
				entry.BindToComponent (component);
				inspectorPanel.entries = inspectorPanel.entries.Add(entry);
				output[i] = entry;
			}
			return output;
		}

		public static InspectorEntry[] AddOrUpdateEntriesAtIndex (_Component component, int typeIndex)
		{
			InspectorEntry[] output = new InspectorEntry[instances.Length];
			HierarchyEntry[] selectedHierarchyEntries = HierarchyPanel.instances[0].selected;
			InspectorEntry entryPrefab = component.inspectorEntryPrefab;
			List<_Component> selectedComponents = new List<_Component>();
			for (int i = 0; i < selectedHierarchyEntries.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selectedHierarchyEntries[i];
				_Component[] obComponents = hierarchyEntry.ob.components;
				int typeCount = 0;
				for (int i2 = 0; i2 < obComponents.Length; i2 ++)
				{
					_Component obComponent = obComponents[i2];
					if (obComponent.inspectorEntryPrefab == entryPrefab)
					{
						if (typeCount == typeIndex)
						{
							selectedComponents.Add(obComponent);
							break;
						}
						typeCount ++;
					}
				}
			}
			if (selectedComponents.Count == 0)
				return output;
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				int?[] ints = new int?[entryPrefab.intValuesEntries.Length];
				float?[] floats = new float?[entryPrefab.floatValuesEntries.Length];
				string[] strings = new string[entryPrefab.stringValuesEntries.Length];
				Color?[] colors = new Color?[entryPrefab.colorValueEntries.Length];
				for (int i2 = 0; i2 < selectedComponents.Count; i2 ++)
				{
					_Component _component = selectedComponents[i2];
					for (int i3 = 0; i3 < ints.Length; i3 ++)
					{
						IntValue intValue = _component.intValues[i3];
						int _i = intValue.val;
						if (i2 == 0)
							ints[i3] = _i;
						else
						{
							int? _i2 = ints[i3];
							if (!_i2.HasValue || _i != _i2.Value)
								ints[i3] = null;
						}
					}
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
				InspectorEntry entry = inspectorPanel.NewEntry(components);
				for (int i2 = 0; i2 < ints.Length; i2 ++)
				{
					int? f = ints[i2];
					if (f == null)
						entry.intValuesEntries[i2].inputField.text = "—";
					else
						entry.intValuesEntries[i2].inputField.text = "" + f;
				}
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