using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Object : MonoBehaviour
	{
		public Transform trs;
		[HideInInspector]
		public _Component[] components = new _Component[0];
		[HideInInspector]
		public Image[] imgs = new Image[0];
	}
}