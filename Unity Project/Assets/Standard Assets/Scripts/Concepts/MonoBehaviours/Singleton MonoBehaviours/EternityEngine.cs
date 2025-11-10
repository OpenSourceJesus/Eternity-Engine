using TMPro;
using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class EternityEngine : SingletonMonoBehaviour<EternityEngine>
	{
		public _Object obPrefab;
		public HierarchyEntry hierarchyEntryPrefab;
		public Image insertionIndicatorPrefab;
		public _Component[] componentsTypesPrefabs = new _Component[0];
		public GameObject  onlyOneComponentPerObjectAllowedNotificationGo;
		public TMP_Text  onlyOneComponentPerObjectAllowedNotificationText;
		static _Object[] obs = new _Object[0];

#if UNITY_EDITOR
		public override void Awake ()
		{
			base.Awake ();
			obs = new _Object[0];
		} 
#endif

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
			_Object ob = Instantiate(obPrefab);
			ob.name = GetUniqueName(name);
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				HierarchyEntry hierarchyEntry = Instantiate(hierarchyEntryPrefab, hierarchyPanel.entriesParent);
				hierarchyEntry.nameText.text = ob.name;
				hierarchyEntry.ob = ob;
				hierarchyEntry.hierarchyPanel = hierarchyPanel;
				hierarchyPanel.entries = hierarchyPanel.entries.Add(hierarchyEntry);
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

		public void AddCompnoenToSelected (int componentTypeIdx)
		{
			HierarchyEntry[] selected = HierarchyPanel.instances[0].selected;
			for (int i = 0; i < selected.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selected[i];
				AddComponent (hierarchyEntry.ob, componentTypeIdx);
			}
		}

		public static string GetUniqueName (string name)
		{
			string origName = name;
			int num = 1;
			while (true)
			{
				bool isValidName = true;
				for (int i = 0; i < obs.Length; i ++)
				{
					_Object ob = obs[i];
					if (ob.name == name)
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