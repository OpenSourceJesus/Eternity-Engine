using Extensions;
using UnityEngine;

namespace EternityEngine
{
	public class _Transform : _Component
	{
		[HideInInspector]
		public Transform trs;
        public Vector3Value pos;
        public FloatValue rot;
        public Vector2Value size;

		void Start ()
		{
			trs = ob.trs;
			pos.onChanged += () => { trs.position = pos.val; };
			rot.onChanged += () => { trs.eulerAngles = Vector3.forward * rot.val; };
			size.onChanged += () => { trs.SetWorldScale (size.val); };
		}

		void OnDestroy ()
		{
			pos.onChanged -= () => { trs.position = pos.val; };
			rot.onChanged -= () => { trs.eulerAngles = Vector3.forward * rot.val; };
			size.onChanged -= () => { trs.SetWorldScale (size.val); };
		}
	}
}