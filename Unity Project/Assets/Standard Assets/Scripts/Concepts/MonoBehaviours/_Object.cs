using UnityEngine;

namespace EternityEngine
{
	public class _Object : MonoBehaviour
	{
		public Transform trs;
		[HideInInspector]
		public _Component[] components = new _Component[0];
	}
}