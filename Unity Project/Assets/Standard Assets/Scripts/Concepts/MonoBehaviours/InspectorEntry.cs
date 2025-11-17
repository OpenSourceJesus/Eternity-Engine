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

		public void SetValueEntries (_Component component)
		{
			for (int i = 0; i < component.floatValues.Length; i ++)
			{
				FloatValue floatValue = component.floatValues[i];
				FloatValueEntry floatValueEntry = floatValuesEntries[i];
				floatValueEntry.value = floatValue;
				floatValue.setableValue = floatValueEntry;
			}
			for (int i = 0; i < component.stringValues.Length; i ++)
			{
				StringValue stringValue = component.stringValues[i];
				StringValueEntry stringValueEntry = stringValuesEntries[i];
				stringValuesEntries[i].value = stringValue;
				stringValue.setableValue = stringValueEntry;
			}
			for (int i = 0; i < component.vector2Values.Length; i ++)
			{
				Vector2Value vector2Value = component.vector2Values[i];
				vector2ValuesEntries[i].value = vector2Value;
			}
			for (int i = 0; i < component.vector3Values.Length; i ++)
			{
				Vector3Value vector3Value = component.vector3Values[i];
				vector3ValuesEntries[i].value = vector3Value;
			}
			this.component = component;
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