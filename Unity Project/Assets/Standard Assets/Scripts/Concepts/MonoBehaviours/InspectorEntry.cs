using TMPro;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace EternityEngine
{
	public class InspectorEntry : MonoBehaviour
	{
		public RectTransform rectTrs;
		[HideInInspector]
		public _Component component;
		public InspectorPanel inspectorPanel;
		int insertAt;
	}
}