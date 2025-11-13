using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class InspectorEntry : MonoBehaviour
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
		int insertAt;

		public void SetValueEntries (_Component component)
		{
			for (int i = 0; i < component.floatValues.Length; i ++)
			{
				FloatValue floatValue = component.floatValues[i];
				floatValuesEntries[i].value = floatValue;
			}
			for (int i = 0; i < component.stringValues.Length; i ++)
			{
				StringValue stringValue = component.stringValues[i];
				stringValuesEntries[i].value = stringValue;
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
	}
}