using Extensions;
using UnityEngine;
using System.Collections.Generic;

namespace EternityEngine
{
	public class _Component : MonoBehaviour
	{
		[HideInInspector]
        public _Object ob;
        public InspectorEntry inspectorEntryPrefab;
		[HideInInspector]
		public bool collapsed;
		public FloatValue[] floatValues = new FloatValue[0];
		public StringValue[] stringValues = new StringValue[0];
		public Vector2Value[] vector2Values = new Vector2Value[0];
		public Vector3Value[] vector3Values = new Vector3Value[0];
		[HideInInspector]
        public InspectorEntry[] inspectorEntries = new InspectorEntry[0];
		public int[] requiredComponentIdxs = new int[0];
		public List<_Component> dependsOn = new List<_Component>();
		public List<_Component> dependsOnMe = new List<_Component>();

		public virtual void Start ()
		{
			List<InspectorEntry> inspectorEntriesPrefabs = new List<InspectorEntry>();
			for (int i = 0; i < ob.components.Length; i ++)
			{
				_Component component = ob.components[i];
				inspectorEntriesPrefabs.Add(component.inspectorEntryPrefab);
			}
			for (int i = 0; i < requiredComponentIdxs.Length; i ++)
			{
				int requiredComponentIdx = requiredComponentIdxs[i];
				_Component componentPrefab = EternityEngine.instance.componentsPrefabs[requiredComponentIdx];
				if (!inspectorEntriesPrefabs.Contains(componentPrefab.inspectorEntryPrefab))
				{
					_Component component = EternityEngine.instance.AddComponent(ob, requiredComponentIdx);
					component.dependsOnMe.Add(this);
					dependsOn.Add(component);
				}
			}
		}

		public bool TryDelete ()
		{
			if (dependsOnMe.Count > 0)
			{
				EternityEngine.instance.cantDeleteComponentNotificationText.text = "Can't remove " + name.Replace("(Clone)", "") + " because " + dependsOnMe[0].name.Replace("(Clone)", "") + " depends on it";
				EternityEngine.instance.cantDeleteComponentNotificationGo.SetActive(true);
				return false;
			}
			for (int i = 0; i < dependsOn.Count; i ++)
			{
				_Component component = dependsOn[i];
				component.dependsOnMe.Remove(this);
			}
			ob.components = ob.components.Remove(this);
			for (int i = 0; i < inspectorEntries.Length; i ++)
			{
				InspectorEntry inspectorEntry = inspectorEntries[i];
				Destroy(inspectorEntry.gameObject);
			}
			Destroy(gameObject);
			return true;
		}
	}
}