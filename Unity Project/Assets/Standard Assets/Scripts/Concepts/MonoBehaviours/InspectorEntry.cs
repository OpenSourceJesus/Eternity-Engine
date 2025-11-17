using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace EternityEngine
{
	public class InspectorEntry : MonoBehaviour, IUpdatable
	{
		public RectTransform rectTrs;
		[HideInInspector]
		public _Component component;
		[HideInInspector]
		public InspectorPanel inspectorPanel;
		public RectTransform collapButtonRectTrs;
		public GameObject goToGetCollapsed;
		public bool onlyAllowOnePerObject;
		public FloatValueEntry[] floatValuesEntries = new FloatValueEntry[0];
		public StringValueEntry[] stringValuesEntries = new StringValueEntry[0];
		public Vector2ValueEntry[] vector2ValuesEntries = new Vector2ValueEntry[0];
		public Vector3ValueEntry[] vector3ValuesEntries = new Vector3ValueEntry[0];
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
			for (int i = 0; i < floatValuesEntries.Length; i ++)
			{
				FloatValueEntry floatValueEntry = floatValuesEntries[i];
				floatValueEntry.DetachValues ();
				Value<float>[] values = new Value<float>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					FloatValue floatValue = component.floatValues[i];
					floatValue.setableValues = floatValue.setableValues.Add(floatValueEntry);
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
					stringValue.setableValues = stringValue.setableValues.Add(stringValueEntry);
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