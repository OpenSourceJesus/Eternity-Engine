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
			if (data == null)
				data = new Data();
		}

		public override void SetData ()
		{
			InitData ();
			base.SetData ();
			SetObIdOfData ();
			SetBoolValuesIdsOfData ();
			SetFloatValuesIdsOfData ();
			SetStringValuesIdsOfData ();
			SetVector2ValuesIdsOfData ();
			SetVector3ValuesIdsOfData ();
			SetColorValuesIdsOfData ();
		}

		void SetObIdOfData ()
		{
			_Data.obId = ob.id;
		}

		void SetObIdFromData ()
		{
			ob = Get<_Object>(_Data.obId);
			if (ob == null)
				ob = (_Object) SaveAndLoadManager.saveData.assetsDatasDict[_Data.obId].GenAsset();
		}

		void SetBoolValuesIdsOfData ()
		{
			_Data.boolValuesIds = new string[boolValues.Length];
			for (int i = 0; i < boolValues.Length; i ++)
			{
				BoolValue boolValue = boolValues[i];
				_Data.boolValuesIds[i] = boolValue.id;
			}
		}

		void SetBoolValuesIdsFromData ()
		{
			for (int i = 0; i < boolValues.Length; i ++)
			{
				BoolValue boolValue = boolValues[i];
				GameManager.assets.Remove(boolValue);
				Destroy(boolValue.gameObject);
			}
			for (int i = 0; i < boolValues.Length; i ++)
			{
				string boolValueId =_Data.boolValuesIds[i];
				BoolValue boolValue = Get<BoolValue>(boolValueId);
				if (boolValue == null)
					boolValue = (BoolValue) SaveAndLoadManager.saveData.assetsDatasDict[boolValueId].GenAsset();
				boolValues[i] = boolValue;
			}
		}

		void SetFloatValuesIdsOfData ()
		{
			_Data.floatValuesIds = new string[floatValues.Length];
			for (int i = 0; i < floatValues.Length; i ++)
			{
				FloatValue floatValue = floatValues[i];
				_Data.floatValuesIds[i] = floatValue.id;
			}
		}

		void SetFloatValuesIdsFromData ()
		{
			for (int i = 0; i < floatValues.Length; i ++)
			{
				FloatValue floatValue = floatValues[i];
				GameManager.assets.Remove(floatValue);
				Destroy(floatValue.gameObject);
			}
			for (int i = 0; i < floatValues.Length; i ++)
			{
				string floatValueId =_Data.floatValuesIds[i];
				FloatValue floatValue = Get<FloatValue>(floatValueId);
				if (floatValue == null)
					floatValue = (FloatValue) SaveAndLoadManager.saveData.assetsDatasDict[floatValueId].GenAsset();
				floatValues[i] = floatValue;
			}
		}

		void SetStringValuesIdsOfData ()
		{
			_Data.stringValuesIds = new string[stringValues.Length];
			for (int i = 0; i < stringValues.Length; i ++)
			{
				StringValue stringValue = stringValues[i];
				_Data.stringValuesIds[i] = stringValue.id;
			}
		}

		void SetStringValuesIdsFromData ()
		{
			for (int i = 0; i < stringValues.Length; i ++)
			{
				StringValue stringValue = stringValues[i];
				GameManager.assets.Remove(stringValue);
				Destroy(stringValue.gameObject);
			}
			for (int i = 0; i < stringValues.Length; i ++)
			{
				string stringValueId =_Data.stringValuesIds[i];
				StringValue stringValue = Get<StringValue>(stringValueId);
				if (stringValue == null)
					stringValue = (StringValue) SaveAndLoadManager.saveData.assetsDatasDict[stringValueId].GenAsset();
				stringValues[i] = stringValue;
			}
		}

		void SetVector2ValuesIdsOfData ()
		{
			_Data.vector2ValuesIds = new string[vector2Values.Length];
			for (int i = 0; i < vector2Values.Length; i ++)
			{
				Vector2Value vector2Value = vector2Values[i];
				_Data.vector2ValuesIds[i] = vector2Value.id;
			}
		}

		void SetVector2ValuesIdsFromData ()
		{
			for (int i = 0; i < vector2Values.Length; i ++)
			{
				Vector2Value vector2Value = vector2Values[i];
				GameManager.assets.Remove(vector2Value);
				Destroy(vector2Value.gameObject);
			}
			for (int i = 0; i < vector2Values.Length; i ++)
			{
				string vector2ValueId =_Data.vector2ValuesIds[i];
				Vector2Value vector2Value = Get<Vector2Value>(vector2ValueId);
				if (vector2Value == null)
					vector2Value = (Vector2Value) SaveAndLoadManager.saveData.assetsDatasDict[vector2ValueId].GenAsset();
				vector2Values[i] = vector2Value;
			}
		}

		void SetVector3ValuesIdsOfData ()
		{
			_Data.vector3ValuesIds = new string[vector3Values.Length];
			for (int i = 0; i < vector3Values.Length; i ++)
			{
				Vector3Value vector3Value = vector3Values[i];
				_Data.vector3ValuesIds[i] = vector3Value.id;
			}
		}

		void SetVector3ValuesIdsFromData ()
		{
			for (int i = 0; i < vector3Values.Length; i ++)
			{
				Vector3Value vector3Value = vector3Values[i];
				GameManager.assets.Remove(vector3Value);
				Destroy(vector3Value.gameObject);
			}
			for (int i = 0; i < vector3Values.Length; i ++)
			{
				string vector3ValueId =_Data.vector3ValuesIds[i];
				Vector3Value vector3Value = Get<Vector3Value>(vector3ValueId);
				if (vector3Value == null)
					vector3Value = (Vector3Value) SaveAndLoadManager.saveData.assetsDatasDict[vector3ValueId].GenAsset();
				vector3Values[i] = vector3Value;
			}
		}

		void SetColorValuesIdsOfData ()
		{
			_Data.colorValuesIds = new string[colorValues.Length];
			for (int i = 0; i < colorValues.Length; i ++)
			{
				ColorValue colorValue = colorValues[i];
				_Data.colorValuesIds[i] = colorValue.id;
			}
		}

		void SetColorValuesIdsFromData ()
		{
			for (int i = 0; i < colorValues.Length; i ++)
			{
				ColorValue colorValue = colorValues[i];
				GameManager.assets.Remove(colorValue);
				Destroy(colorValue.gameObject);
			}
			for (int i = 0; i < colorValues.Length; i ++)
			{
				string colorValueId =_Data.colorValuesIds[i];
				ColorValue colorValue = Get<ColorValue>(colorValueId);
				if (colorValue == null)
					colorValue = (ColorValue) SaveAndLoadManager.saveData.assetsDatasDict[colorValueId].GenAsset();
				colorValues[i] = colorValue;
			}
		}

		[Serializable]
		public class Data : Asset.Data
		{
			public string obId;
			public string[] boolValuesIds = new string[0];
			public string[] floatValuesIds = new string[0];
			public string[] stringValuesIds = new string[0];
			public string[] vector2ValuesIds = new string[0];
			public string[] vector3ValuesIds = new string[0];
			public string[] colorValuesIds = new string[0];

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				_Component component = (_Component) asset;
				component.SetObIdFromData ();
				component.SetBoolValuesIdsFromData ();
				component.SetFloatValuesIdsFromData ();
				component.SetStringValuesIdsFromData ();
				component.SetVector2ValuesIdsFromData ();
				component.SetVector3ValuesIdsFromData ();
				component.SetColorValuesIdsFromData ();
			}
		}
	}
}