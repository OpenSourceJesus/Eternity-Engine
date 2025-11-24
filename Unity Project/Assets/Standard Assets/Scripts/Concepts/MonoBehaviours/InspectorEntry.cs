using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace EternityEngine
{
	public class InspectorEntry : Asset, IUpdatable
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
		public RectTransform rectTrs;
		[HideInInspector]
		public _Component component;
		[HideInInspector]
		public InspectorPanel inspectorPanel;
		public RectTransform collapButtonRectTrs;
		public GameObject goToGetCollapsed;
		public bool onlyAllowOnePerObject;
		public BoolValueEntry[] boolValuesEntries = new BoolValueEntry[0];
		public FloatValueEntry[] floatValuesEntries = new FloatValueEntry[0];
		public StringValueEntry[] stringValuesEntries = new StringValueEntry[0];
		public Vector2ValueEntry[] vector2ValuesEntries = new Vector2ValueEntry[0];
		public Vector3ValueEntry[] vector3ValuesEntries = new Vector3ValueEntry[0];
		public ColorValueEntry[] colorValueEntries = new ColorValueEntry[0];
		public RectTransform optionsRectTrs;
		OptionsUpdater optionsUpdater;
		int insertAt;

		public void OnMouseEnter ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void OnMouseExit ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public void DoUpdate ()
		{
			if (Mouse.current.rightButton.wasPressedThisFrame)
				ToggleOptions ();
		}

		public void SetValueEntries (params _Component[] components)
		{
			_Component firstComponent = components[0];
			for (int i = 0; i < boolValuesEntries.Length; i ++)
			{
				BoolValueEntry boolValuesEntry = boolValuesEntries[i];
				boolValuesEntry.DetachValues ();
				Value<bool>[] values = new Value<bool>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					values[i2] = component.boolValues[i];
				}
				boolValuesEntry.SetValues (values);
			}
			for (int i = 0; i < floatValuesEntries.Length; i ++)
			{
				FloatValueEntry floatValueEntry = floatValuesEntries[i];
				floatValueEntry.DetachValues ();
				Value<float>[] values = new Value<float>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					FloatValue floatValue = component.floatValues[i];
					floatValue.setters = floatValue.setters.Add(floatValueEntry);
					values[i2] = floatValue;
				}
				floatValueEntry.SetValues (values);
			}
			for (int i = 0; i < stringValuesEntries.Length; i ++)
			{
				StringValueEntry stringValueEntry = stringValuesEntries[i];
				stringValueEntry.DetachValues ();
				Value<string>[] values = new Value<string>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					StringValue stringValue = component.stringValues[i];
					stringValue.setters = stringValue.setters.Add(stringValueEntry);
					values[i2] = stringValue;
				}
				stringValueEntry.SetValues (values);
			}
			for (int i = 0; i < vector2ValuesEntries.Length; i ++)
			{
				Vector2ValueEntry vector2ValueEntry = vector2ValuesEntries[i];
				vector2ValueEntry.DetachValues ();
				Value<Vector2>[] values = new Value<Vector2>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					values[i2] = component.vector2Values[i];
				}
				vector2ValueEntry.SetValues (values);
			}
			for (int i = 0; i < vector3ValuesEntries.Length; i ++)
			{
				Vector3ValueEntry vector3ValueEntry = vector3ValuesEntries[i];
				vector3ValueEntry.DetachValues ();
				Value<Vector3>[] values = new Value<Vector3>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					values[i2] = component.vector3Values[i];
				}
				vector3ValueEntry.SetValues (values);
			}
			for (int i = 0; i < colorValueEntries.Length; i ++)
			{
				ColorValueEntry colorValueEntry = colorValueEntries[i];
				colorValueEntry.DetachValues ();
				Value<Color>[] values = new Value<Color>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					values[i2] = component.colorValues[i];
				}
				colorValueEntry.SetValues (values);
			}
			component = firstComponent;
		}

		public void SetCollapsed (bool collapse)
		{
			goToGetCollapsed.SetActive(!collapse);
			collapButtonRectTrs.eulerAngles = Vector3.forward * 180 * collapse.GetHashCode();
			component.collapsed = collapse;
		}

		public void ToggleCollapse ()
		{
			SetCollapsed (!component.collapsed);
		}

		public void TryDelete ()
		{
			if (component.TryDelete())
			{
				int idx = rectTrs.GetSiblingIndex();
				for (int i = 0; i < InspectorPanel.instances.Length; i ++)
				{
					InspectorPanel inspectorPanel = InspectorPanel.instances[i];
					inspectorPanel.entries = inspectorPanel.entries.RemoveAt(idx);
				}
				Destroy(gameObject);
			}
		}

		public void ToggleOptions ()
		{
			optionsRectTrs.gameObject.SetActive(!optionsRectTrs.gameObject.activeSelf);
			if (optionsRectTrs.gameObject.activeSelf)
			{
				optionsUpdater = new OptionsUpdater(this);
				GameManager.updatables = GameManager.updatables.Add(optionsUpdater);
			}
			else
				GameManager.updatables = GameManager.updatables.Remove(optionsUpdater);
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
			SetActiveOfData ();
			SetComponentIdOfData ();
			SetCollapsedOfData ();
		}

		void SetActiveOfData ()
		{
			_Data.active = gameObject.activeSelf;
		}

		void SetActiveFromData ()
		{
			gameObject.SetActive(_Data.active);
		}

		void SetComponentIdOfData ()
		{
			_Data.componentId = component.id;
		}

		void SetComponentIdFromData ()
		{
			_Component component = Get<_Component>(_Data.componentId);
			if (component == null)
				component = (_Component) SaveAndLoadManager.saveData.assetsDatasDict[_Data.componentId].GenAsset();
		}

		void SetCollapsedOfData ()
		{
			_Data.collapsed = component.collapsed;
		}

		void SetCollapsedFromData ()
		{
			SetCollapsed (_Data.collapsed);
		}

		[Serializable]
		public class Data : Asset.Data
		{
			public bool active;
			public string componentId;
			public bool collapsed;

			public override object GenAsset ()
			{
				_Component component = Get<_Component>(componentId);
				if (component == null)
					component = (_Component) SaveAndLoadManager.saveData.assetsDatasDict[componentId].GenAsset();
				InspectorEntry inspectorEntryPrefab = EternityEngine.instance.obDataEntryPrefab;
				for (int i = 0; i < EternityEngine.instance.componentsPrefabs.Length; i ++)
				{
					_Component componentPrefab = EternityEngine.instance.componentsPrefabs[i];
					if (component.inspectorEntryPrefab == componentPrefab.inspectorEntryPrefab)
					{
						inspectorEntryPrefab = component.inspectorEntryPrefab;
						break;
					}
				}
				InspectorPanel firstInspectorPanel = InspectorPanel.instances[0];
				InspectorEntry inspectorEntry = Instantiate(inspectorEntryPrefab, firstInspectorPanel.entriesParent);
				Apply (inspectorEntry);
				inspectorEntry.inspectorPanel = firstInspectorPanel;
				firstInspectorPanel.entries = firstInspectorPanel.entries.Add(inspectorEntry);
				for (int i = 1; i < InspectorPanel.instances.Length; i ++)
				{
					InspectorPanel inspectorPanel = InspectorPanel.instances[i];
					inspectorEntry = Instantiate(inspectorEntry, inspectorPanel.entriesParent);
					inspectorEntry.save = false;
					inspectorEntry.inspectorPanel = inspectorPanel;
					inspectorPanel.entries = inspectorPanel.entries.Add(inspectorEntry);
				}
				return inspectorEntry;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				InspectorEntry inspectorEntry = (InspectorEntry) asset;
				inspectorEntry.SetComponentIdFromData ();
				inspectorEntry.SetActiveFromData ();
				inspectorEntry.SetCollapsedFromData ();
			}
		}

		class OptionsUpdater : IUpdatable
		{
			InspectorEntry inspectorEntry;
			bool prevClicking = true;

			public OptionsUpdater (InspectorEntry inspectorEntry)
			{
				this.inspectorEntry = inspectorEntry;
			}

			public void DoUpdate ()
			{
				bool clicking = Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed;
				if (clicking && !prevClicking)
				{
					Vector2 mousePos = Mouse.current.position.ReadValue();
					Rect optionssWorldRect = inspectorEntry.optionsRectTrs.GetWorldRect();
					if (!optionssWorldRect.Contains(mousePos))
					{
						inspectorEntry.optionsRectTrs.gameObject.SetActive(false);
						GameManager.updatables = GameManager.updatables.Remove(this);
					}
				}
				prevClicking = clicking;
			}
		}
	}
}