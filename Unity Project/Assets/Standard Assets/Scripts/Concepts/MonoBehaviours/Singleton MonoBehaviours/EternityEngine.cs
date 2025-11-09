using System;
using Extensions;
using UnityEngine;

namespace EternityEngine
{
	public class EternityEngine : SingletonMonoBehaviour<EternityEngine>
	{
		public _Object obPrefab;
		public HierarchyEntry hierarchyEntryPrefab;
		public RectTransform insertionIndicatorPrefab;
		static _Object[] obs = new _Object[0];

#if UNITY_EDITOR
		void Awake ()
		{
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