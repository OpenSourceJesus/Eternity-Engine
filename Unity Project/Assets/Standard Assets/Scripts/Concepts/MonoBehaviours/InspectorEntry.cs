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
		public Vector3ValueEntry[] vector3ValuesEntries = new Vector3ValueEntry[0];
		int insertAt;

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