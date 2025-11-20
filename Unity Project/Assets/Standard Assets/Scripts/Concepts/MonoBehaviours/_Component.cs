using System;
using Extensions;
using UnityEngine;
using System.Collections.Generic;

namespace EternityEngine
{
	public class _Component : Asset
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
		public SceneEntry sceneEntry;
		[HideInInspector]
        public _Object ob;
        public InspectorEntry inspectorEntryPrefab;
		[HideInInspector]
		public bool collapsed;
		public BoolValue[] boolValues = new BoolValue[0];
		public FloatValue[] floatValues = new FloatValue[0];
		public StringValue[] stringValues = new StringValue[0];
		public Vector2Value[] vector2Values = new Vector2Value[0];
		public Vector3Value[] vector3Values = new Vector3Value[0];
		public ColorValue[] colorValues = new ColorValue[0];
		[HideInInspector]
        public InspectorEntry[] inspectorEntries = new InspectorEntry[0];
		[HideInInspector]
        public SceneEntry[] sceneEntries = new SceneEntry[0];
		public int[] requiredComponentIdxs = new int[0];
		public List<_Component> dependsOn = new List<_Component>();
		public List<_Component> dependsOnMe = new List<_Component>();

		public virtual void Init ()
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

		public override void Awake ()
		{
			base.Awake ();
			EternityEngine.components = EternityEngine.components.Add(this);
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			EternityEngine.components = EternityEngine.components.Remove(this);
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

		public override void InitData ()
		{
			if (_Data == null)
				_Data = new Data();
		}

		public override void SetData ()
		{
			base.SetData ();
			SetObNameOFData ();
		}

		void SetObNameOFData ()
		{
			_Data.obName = ob.name;
		}

		void SetObNameFromData ()
		{
			for (int i = 0; i < EternityEngine.obs.Length; i ++)
			{
				_Object ob = EternityEngine.obs[i];
				if (ob.name == _Data.obName)
				{
					this.ob = ob;
					return;
				}
			}
			ob = Get<_Object>(_Data.obName);
		}

		[Serializable]
		public class Data : Asset.Data
		{
			public string obName;

			public override object GenAsset ()
			{
				_Component component = Instantiate(EternityEngine.instance.componentPrefab);
				Apply (component);
				return component;
			}

			public override void Apply (Asset asset)
			{
				base.Apply (asset);
				_Component component = (_Component) asset;
				component._Data = this;
				component.SetObNameFromData ();
			}
		}
	}
}