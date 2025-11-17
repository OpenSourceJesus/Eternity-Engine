using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Object : MonoBehaviour
	{
		[HideInInspector]
		public _Component[] components = new _Component[0];
		[HideInInspector]
		public SceneEntry[] sceneEntries = new SceneEntry[0];
		[HideInInspector]
		public HierarchyEntry[] hierarchyEntries = new HierarchyEntry[0];
	}
}