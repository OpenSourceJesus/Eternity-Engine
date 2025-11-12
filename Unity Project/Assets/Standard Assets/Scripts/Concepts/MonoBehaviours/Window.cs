using UnityEngine;

namespace EternityEngine
{
	public class Window : MonoBehaviour
	{
		public RectTransform rectTrs;
		public Rect initScreenNormalizedRect;
		public RectTransform canvasRectTrs;

		public virtual void Awake ()
		{
			rectTrs.sizeDelta = canvasRectTrs.sizeDelta * initScreenNormalizedRect.size;
			rectTrs.position = canvasRectTrs.sizeDelta * initScreenNormalizedRect.center;
        }
	}
}