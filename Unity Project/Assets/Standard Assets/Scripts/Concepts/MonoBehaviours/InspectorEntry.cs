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
		int insertAt;
		bool collapsed;

		public void SetCollapsed (bool collapse)
		{
			goToGetCollapsed.SetActive(!collapse);
			collapButtonRectTrs.eulerAngles = Vector3.forward * 180 * collapse.GetHashCode();
			collapsed = collapse;
		}

		public void ToggleCollapse ()
		{
			SetCollapsed (!collapsed);
		}
	}
}