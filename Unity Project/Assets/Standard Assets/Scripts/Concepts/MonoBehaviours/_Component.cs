using System;
using Extensions;
using UnityEngine;
using System.Collections.Generic;

namespace EternityEngine
{
	public class _Component : MonoBehaviour
	{
		public object data;
		public Data _Data
		{
			get
			{
				if (data == null)
					data = new Data();
				return (Data) data;
			}
			set
			{
				data = value;
			}
		}
		public int prefabIdx;
		public SceneEntry sceneEntry;
        public _Object ob;
        public InspectorEntry inspectorEntryPrefab;
		[HideInInspector]
		public bool collapsed;
		public BoolValue[] boolValues = new BoolValue[0];
		public IntValue[] intValues = new IntValue[0];
		public FloatValue[] floatValues = new FloatValue[0];
		public StringValue[] stringValues = new StringValue[0];
		public Vector2Value[] vector2Values = new Vector2Value[0];
		public Vector3Value[] vector3Values = new Vector3Value[0];
		public ColorValue[] colorValues = new ColorValue[0];
		public BoolArrayValue[] boolArrayValues = new BoolArrayValue[0];
		public IntArrayValue[] intArrayValues = new IntArrayValue[0];
		public FloatArrayValue[] floatArrayValues = new FloatArrayValue[0];
		public Vector2ArrayValue[] vector2ArrayValues = new Vector2ArrayValue[0];
		public BoolAttributeValue[] boolAttributeValues = new BoolAttributeValue[0];
		public StringAttributeValue[] stringAttributeValues = new StringAttributeValue[0];
		public Vector2AttributeValue[] vector2AttributeValues = new Vector2AttributeValue[0];
		public Vector3AttributeValue[] vector3AttributeValues = new Vector3AttributeValue[0];
		public ColorAttributeValue[] colorAttributeValues = new ColorAttributeValue[0];
		[HideInInspector]
        public InspectorEntry[] inspectorEntries = new InspectorEntry[0];
		[HideInInspector]
        public SceneEntry[] sceneEntries = new SceneEntry[0];
		public int[] requiredComponentsIdxs = new int[0];
		[HideInInspector]
		public List<_Component> dependsOn = new List<_Component>();
		[HideInInspector]
		public List<_Component> dependsOnMe = new List<_Component>();

		public virtual void Init ()
		{
			if (!SaveAndLoadManager.isLoading)
			{
				List<InspectorEntry> inspectorEntriesPrefabs = new List<InspectorEntry>();
				for (int i = 0; i < ob.components.Length; i ++)
				{
					_Component component = ob.components[i];
					inspectorEntriesPrefabs.Add(component.inspectorEntryPrefab);
				}
				for (int i = 0; i < requiredComponentsIdxs.Length; i ++)
				{
					int requiredComponentIdx = requiredComponentsIdxs[i];
					_Component componentPrefab = EternityEngine.instance.componentsPrefabs[requiredComponentIdx];
					if (!inspectorEntriesPrefabs.Contains(componentPrefab.inspectorEntryPrefab))
					{
						_Component component = EternityEngine.instance.AddComponent(ob, requiredComponentIdx);
						component.dependsOnMe.Add(this);
						dependsOn.Add(component);
					}
				}
			}
			if (sceneEntry == null)
				return;
			ScenePanel firstScenePanel = ScenePanel.instances[0];
			sceneEntry.rectTrs.SetParent(firstScenePanel.obsParentRectTrs);
			sceneEntries = new SceneEntry[ScenePanel.instances.Length];
			sceneEntries[0] = sceneEntry;
			firstScenePanel.entries = firstScenePanel.entries.Add(sceneEntry);
			sceneEntry.hierarchyEntries = ob.hierarchyEntries;
			sceneEntry.scenePanel = firstScenePanel;
			for (int i = 1; i < ScenePanel.instances.Length; i ++)
			{
				ScenePanel scenePanel = ScenePanel.instances[i];
				SceneEntry _sceneEntry = Instantiate(sceneEntry, scenePanel.obsParentRectTrs);
				scenePanel.entries = scenePanel.entries.Add(_sceneEntry);
				sceneEntry.scenePanel = scenePanel;
			}
			ob.sceneEntries = ob.sceneEntries.AddRange(sceneEntries);
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
			for (int i = 0; i < sceneEntries.Length; i ++)
			{
				SceneEntry sceneEntry = sceneEntries[i];
				ScenePanel scenePanel = ScenePanel.instances[i];
				scenePanel.entries = scenePanel.entries.Remove(sceneEntry);
				Destroy(sceneEntry.gameObject);
				ob.sceneEntries = ob.sceneEntries.Remove(sceneEntry);
			}
			Destroy(gameObject);
			return true;
		}

		public (Data data, _Component component) GetDataAndComponent ()
		{
			ObjectData obData = this as ObjectData;
			if (obData != null)
				return (obData._Data, obData);
			_Transform trs = this as _Transform;
			if (trs != null)
				return (trs._Data, trs);
			_Image img = this as _Image;
			if (img != null)
				return (img._Data, img);
			RigidBody rigidBody = this as RigidBody;
			if (rigidBody != null)
				return (rigidBody._Data, rigidBody);
			_Collider collider = this as _Collider;
			if (collider != null)
				return (collider._Data, collider);
			Script script = this as Script;
			if (script != null)
				return (script._Data, script);
			return (null, null);
		}

		[Serializable]
		public class Data
		{
			public int prefabIdx;
			
			public virtual void Set (_Component component)
			{
				prefabIdx = component.prefabIdx;
			}

			public virtual void Apply (_Component component)
			{
				component.prefabIdx = prefabIdx;
			}
		}
	}
}