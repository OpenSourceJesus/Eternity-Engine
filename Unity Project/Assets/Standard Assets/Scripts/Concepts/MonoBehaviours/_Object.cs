using UnityEngine;

namespace EternityEngine
{
	public class _Object : MonoBehaviour
	{
		[HideInInspector]
		public _Component[] components = new _Component[0];
	}
}