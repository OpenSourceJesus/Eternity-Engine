using UnityEngine;

namespace EternityEngine
{
	public class Window : MonoBehaviour
	{
		public RectTransform rectTrs;
		public Rect initScreenNormalizedRect;

		public virtual void Awake ()
		{
			rectTrs.sizeDelta = EternityEngine.instance.canvasRectTrs.sizeDelta * initScreenNormalizedRect.size;
			rectTrs.position = EternityEngine.instance.canvasRectTrs.sizeDelta * initScreenNormalizedRect.center;
        }
	}
}