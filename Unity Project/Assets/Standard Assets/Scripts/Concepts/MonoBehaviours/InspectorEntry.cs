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
		public _Component[] components = new _Component[0];
		[HideInInspector]
		public InspectorPanel inspectorPanel;
		public RectTransform collapseButtonRectTrs;
		public GameObject goToGetCollapsed;
		public bool onlyAllowOnePerObject;
		public BoolValueEntry[] boolValuesEntries = new BoolValueEntry[0];
		public IntValueEntry[] intValuesEntries = new IntValueEntry[0];
		public FloatValueEntry[] floatValuesEntries = new FloatValueEntry[0];
		public StringValueEntry[] stringValuesEntries = new StringValueEntry[0];
		public Vector2ValueEntry[] vector2ValuesEntries = new Vector2ValueEntry[0];
		public Vector3ValueEntry[] vector3ValuesEntries = new Vector3ValueEntry[0];
		public ColorValueEntry[] colorValueEntries = new ColorValueEntry[0];
		public EnumValueEntry[] enumValueEntries = new EnumValueEntry[0];
		public BoolArrayValueEntry[] boolArrayValuesEntries = new BoolArrayValueEntry[0];
		public IntArrayValueEntry[] intArrayValuesEntries = new IntArrayValueEntry[0];
		public FloatArrayValueEntry[] floatArrayValuesEntries = new FloatArrayValueEntry[0];
		public Vector2ArrayValueEntry[] vector2ArrayValuesEntries = new Vector2ArrayValueEntry[0];
		public BoolAttributeValueEntry[] boolAttributeValuesEntries = new BoolAttributeValueEntry[0];
		public StringAttributeValueEntry[] stringAttributeValuesEntries = new StringAttributeValueEntry[0];
		public Vector2AttributeValueEntry[] vector2AttributeValuesEntries = new Vector2AttributeValueEntry[0];
		public Vector3AttributeValueEntry[] vector3AttributeValuesEntries = new Vector3AttributeValueEntry[0];
		public ColorAttributeValueEntry[] colorAttributeValuesEntries = new ColorAttributeValueEntry[0];
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

		public void BindToComponent (_Component component)
		{
			SetValueEntries (component);
			UpdateDisplay (component);
		}

		public virtual void UpdateDisplay (_Component component)
		{
			for (int i = 0; i < boolValuesEntries.Length; i ++)
				boolValuesEntries[i].UpdateDisplay (component.boolValues[i].val);
			for (int i = 0; i < intValuesEntries.Length; i ++)
			{
				IntValue intValue = component.intValues[i];
				if (i < enumValueEntries.Length)
					enumValueEntries[i].UpdateDisplay (intValue.val);
				intValuesEntries[i].UpdateDisplay (intValue.val);
			}
			for (int i = 0; i < floatValuesEntries.Length; i ++)
				floatValuesEntries[i].UpdateDisplay (component.floatValues[i].val);
			for (int i = 0; i < stringValuesEntries.Length; i ++)
				stringValuesEntries[i].UpdateDisplay (component.stringValues[i].val);
			for (int i = 0; i < vector2ValuesEntries.Length; i ++)
				vector2ValuesEntries[i].UpdateDisplay (component.vector2Values[i].val);
			for (int i = 0; i < vector3ValuesEntries.Length; i ++)
				vector3ValuesEntries[i].UpdateDisplay( component.vector3Values[i].val);
			for (int i = 0; i < colorValueEntries.Length; i ++)
				colorValueEntries[i].UpdateDisplay (component.colorValues[i].val);
			for (int i = 0; i < boolArrayValuesEntries.Length; i ++)
				boolArrayValuesEntries[i].UpdateDisplay (component.boolArrayValues[i].val);
			for (int i = 0; i < intArrayValuesEntries.Length; i ++)
				intArrayValuesEntries[i].UpdateDisplay (component.intArrayValues[i].val);
			for (int i = 0; i < floatArrayValuesEntries.Length; i ++)
				floatArrayValuesEntries[i].UpdateDisplay (component.floatArrayValues[i].val);
			for (int i = 0; i < vector2ArrayValuesEntries.Length; i ++)
				vector2ArrayValuesEntries[i].UpdateDisplay (component.vector2ArrayValues[i].val);
		}

		public void SetValueEntries (params _Component[] components)
		{
			this.components = components;
			_Component firstComponent = components[0];
			for (int i = 0; i < boolValuesEntries.Length; i ++)
			{
				BoolValueEntry boolValueEntry = boolValuesEntries[i];
				boolValueEntry.DetachValues ();
				Value<bool>[] values = new Value<bool>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					values[i2] = component.boolValues[i];
				}
				boolValueEntry.SetValues (values);
			}
			for (int i = 0; i < intValuesEntries.Length; i ++)
			{
				IntValueEntry intValueEntry = intValuesEntries[i];
				intValueEntry.DetachValues ();
				Value<int>[] values = new Value<int>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					IntValue intValue = component.intValues[i];
					intValue.entries = intValue.entries.Add(intValueEntry);
					values[i2] = intValue;
				}
				intValueEntry.SetValues (values);
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
					floatValue.entries = floatValue.entries.Add(floatValueEntry);
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
					stringValue.entries = stringValue.entries.Add(stringValueEntry);
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
			for (int i = 0; i < boolArrayValuesEntries.Length; i ++)
			{
				BoolArrayValueEntry boolArrayValueEntry = boolArrayValuesEntries[i];
				boolArrayValueEntry.DetachValues ();
				Value<bool[]>[] values = new Value<bool[]>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					values[i2] = component.boolArrayValues[i];
				}
				boolArrayValueEntry.SetValues (values);
			}
			for (int i = 0; i < intArrayValuesEntries.Length; i ++)
			{
				IntArrayValueEntry intArrayValueEntry = intArrayValuesEntries[i];
				intArrayValueEntry.DetachValues ();
				Value<int[]>[] values = new Value<int[]>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					values[i2] = component.intArrayValues[i];
				}
				intArrayValueEntry.SetValues (values);
			}
			for (int i = 0; i < floatArrayValuesEntries.Length; i ++)
			{
				FloatArrayValueEntry floatArrayValueEntry = floatArrayValuesEntries[i];
				floatArrayValueEntry.DetachValues ();
				Value<float[]>[] values = new Value<float[]>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					values[i2] = component.floatArrayValues[i];
				}
				floatArrayValueEntry.SetValues (values);
			}
			for (int i = 0; i < vector2ArrayValuesEntries.Length; i ++)
			{
				Vector2ArrayValueEntry vector2ArrayValueEntry = vector2ArrayValuesEntries[i];
				vector2ArrayValueEntry.DetachValues ();
				Value<Vector2[]>[] values = new Value<Vector2[]>[components.Length];
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					values[i2] = component.vector2ArrayValues[i];
				}
				vector2ArrayValueEntry.SetValues (values);
			}
			component = firstComponent;
		}

		public void SetCollapsed (bool collapse)
		{
			goToGetCollapsed.SetActive(!collapse);
			collapseButtonRectTrs.eulerAngles = Vector3.forward * 180 * collapse.GetHashCode();
			component.collapsed = collapse;
		}

		public void ToggleCollapse ()
		{
			SetCollapsed (!component.collapsed);
		}

		public void TryDelete ()
		{
			bool allDeleted = true;
			for (int i = 0; i < components.Length; i ++)
			{
				_Component deleteComponent = components[i];
				if (!deleteComponent.TryDelete())
					allDeleted = false;
			}
			if (allDeleted)
			{
				int idx = inspectorPanel.entries.IndexOf(this);
				for (int i = 0; i < InspectorPanel.instances.Length; i ++)
				{
					InspectorPanel _inspectorPanel = InspectorPanel.instances[i];
					_inspectorPanel.entries = _inspectorPanel.entries.RemoveAt(idx);
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