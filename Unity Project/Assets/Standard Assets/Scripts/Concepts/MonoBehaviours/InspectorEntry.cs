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
		int insertAt;
	}
}