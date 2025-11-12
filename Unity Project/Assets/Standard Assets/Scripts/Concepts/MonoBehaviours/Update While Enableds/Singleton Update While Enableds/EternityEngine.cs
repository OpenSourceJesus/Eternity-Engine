using TMPro;
using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace EternityEngine
{
	public class EternityEngine : SingletonUpdateWhileEnabled<EternityEngine>
	{
		public _Object obPrefab;
		public HierarchyEntry hierarchyEntryPrefab;
		public Image insertionIndicatorPrefab;
		public _Component[] componentsTypesPrefabs = new _Component[0];
		public GameObject  onlyOneComponentPerObjectAllowedNotificationGo;
		public TMP_Text  onlyOneComponentPerObjectAllowedNotificationText;
		static _Object[] obs = new _Object[0];
		static bool prevDoDuplicate;

#if UNTIY_EDITOR
		public override void Awake ()
		{
			base.Awake ();
			obs = new _Object[0];
		}
#endif

		public override void DoUpdate ()
		{
			bool doDuplicate = Keyboard.current.dKey.isPressed && Keyboard.current.leftCtrlKey.isPressed;
			if (doDuplicate && !prevDoDuplicate)
			{
				HierarchyPanel firstHierarchyPanel = HierarchyPanel.instances[0];
				HierarchyEntry[] selected = firstHierarchyPanel.selected._Sort(new HierarchyEntry.Comparer());
				Dictionary<int, int> hierarchyEntriesIdxsDict = new Dictionary<int, int>();
				for (int i = 0; i < selected.Length; i ++)
				{
					HierarchyEntry hierarchyEntry = selected[i];
					string name = hierarchyEntry.ob.name;
					if (name.EndsWith(')'))
					{
						int spaceAndLeftParenthesisIdx = name.LastIndexOf(" (");
						int val;
						if (spaceAndLeftParenthesisIdx != -1 && int.TryParse(name.SubstringStartEnd(spaceAndLeftParenthesisIdx + 2, name.Length - 2), out val))
							name = name.Remove(spaceAndLeftParenthesisIdx);
					}
					NewObject (hierarchyEntry.ob, name);
					hierarchyEntriesIdxsDict[firstHierarchyPanel.entries.Length - 1] = hierarchyEntry.rectTrs.GetSiblingIndex() + 1 + hierarchyEntriesIdxsDict.Count;
				}
				for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
				{
					HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
					for (int i2 = 0; i2 < hierarchyPanel.selected.Length; i2 ++)
					{
						HierarchyEntry hierarchyEntry = hierarchyPanel.selected[i2];
						hierarchyEntry.SetSelected (false);
						i2 --;
					}
					foreach (KeyValuePair<int, int> keyValuePair in hierarchyEntriesIdxsDict)
					{
						HierarchyEntry hierarchyEntry = hierarchyPanel.entries[keyValuePair.Key];
						hierarchyEntry.Reorder (keyValuePair.Value);
						hierarchyEntry.SetSelected (true);
					}
				}
			}
			prevDoDuplicate = doDuplicate;
			if (Keyboard.current.f2Key.wasPressedThisFrame)
			{
				for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
				{
					HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
					HierarchyEntry[] selected = hierarchyPanel.selected;
					if (selected.Length > 0)
					{
						if (selected.Length == 1)
						{
							HierarchyEntry hierarchyEntry = selected[0];
							hierarchyEntry.nameInputField.gameObject.SetActive(true);
							hierarchyEntry.nameInputField.text = hierarchyEntry.ob.name;
						}
						else
						{
							string nameOverlap = selected[0].ob.name.GetOverlapFromStart(selected[1].ob.name);
							for (int i2 = 2; i2 < selected.Length; i2 ++)
							{
								HierarchyEntry hierarchyEntry = selected[i2];
								nameOverlap = hierarchyEntry.ob.name.GetOverlapFromStart(nameOverlap);
							}
							for (int i2 = 0; i2 < selected.Length; i2 ++)
							{
								HierarchyEntry hierarchyEntry = selected[i2];
								hierarchyEntry.nameInputField.gameObject.SetActive(true);
								hierarchyEntry.nameInputField.text = nameOverlap;
							}
						}
					}
				}
			}
		}

		public void NewPresetObject (int presetObjectTypeVal)
		{
			NewPresetObject ((PresetObjectType) Enum.ToObject(typeof(PresetObjectType), presetObjectTypeVal));
		}

		public _Object NewPresetObject (PresetObjectType presetObjectType)
		{
			if (presetObjectType == PresetObjectType.Empty)
				return NewObject();
			else
				throw new NotImplementedException();
		}

		public _Object NewObject (string name = "Object", Vector2 pos = new Vector2(), float rot = 0)
		{
			_Object ob = NewObject(obPrefab, name);

			return ob;
		}

		public _Object NewObject (_Object template, string name = "Object")
		{
			_Object ob = Instantiate(template);
			ob.name = GetUniqueName(name);
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				HierarchyEntry hierarchyEntry = Instantiate(EternityEngine.instance.hierarchyEntryPrefab, hierarchyPanel.entriesParent);
				hierarchyEntry.nameText.text = ob.name;
				hierarchyEntry.ob = ob;
				hierarchyEntry.hierarchyPanel = hierarchyPanel;
				hierarchyPanel.entries = hierarchyPanel.entries.Add(hierarchyEntry);
			}
			ob.components = new _Component[template.components.Length];
			for (int i = 0; i < template.components.Length; i ++)
			{
				_Component component = template.components[i];
				component = Instantiate(component);
				for (int i2 = 0; i2 < component.inspectorEntries.Length; i2 ++)
				{
					InspectorEntry inspectorEntry = component.inspectorEntries[i2];
					inspectorEntry = Instantiate(inspectorEntry, inspectorEntry.rectTrs.parent);
					inspectorEntry.gameObject.SetActive(false);
					component.inspectorEntries[i2] = inspectorEntry;
				}
				ob.components[i] = component;
			}
			obs = obs.Add(ob);
			return ob;
		}

		public _Component AddComponent (_Object ob, int componentTypeIdx)
		{
			_Component componentPrefab = componentsTypesPrefabs[componentTypeIdx];
			for (int i = 0; i < ob.components.Length; i ++)
			{
				_Component _component = ob.components[i];
				if (_component.inspectorEntryPrefab.onlyAllowOnePerObject && componentPrefab.inspectorEntryPrefab == _component.inspectorEntryPrefab)
				{
					instance.onlyOneComponentPerObjectAllowedNotificationText.text = "There can't be multiple " + componentPrefab.name +  " components attached to the same object. " + ob.name +  " already contains a " + componentPrefab.name + " component.";
					instance.onlyOneComponentPerObjectAllowedNotificationGo.SetActive(true);
					return null;
				}
			}
			_Component component = Instantiate(componentPrefab);
			component.ob = ob;
			ob.components = ob.components.Add(component);
			InspectorPanel.AddEntries (component);
			return component;
		}

		public void AddCompnoentToSelected (int componentTypeIdx)
		{
			HierarchyEntry[] selected = HierarchyPanel.instances[0].selected;
			for (int i = 0; i < selected.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selected[i];
				AddComponent (hierarchyEntry.ob, componentTypeIdx);
			}
		}

		public static string GetUniqueName (string name, params _Object[] excludeObs)
		{
			string origName = name;
			int num = 1;
			while (true)
			{
				bool isValidName = true;
				for (int i = 0; i < obs.Length; i ++)
				{
					_Object ob = obs[i];
					if (ob.name == name && !excludeObs.Contains(ob))
					{
						isValidName = false;
						break;
					}
				}
				if (isValidName)
					return name;
				else
				{
					name = origName + " (" + num + ')';
					num ++;
				}
			}
		}

		public enum PresetObjectType
		{
			Empty,
			Image,
			ParticleSystem
		}
	}
}