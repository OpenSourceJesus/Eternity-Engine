using UnityEngine;

namespace EternityEngine
{
	public class _Transform : _Component
	{
        public Vector3Value pos;
        public FloatValue rot;

		void Awake ()
		{
			pos.onChanged += () => { ob.trs.position = pos.val; };
			rot.onChanged += () => { ob.trs.eulerAngles = Vector3.forward * rot.val; };
		}

		void OnDestroy ()
		{
			pos.onChanged -= () => { ob.trs.position = pos.val; };
			rot.onChanged -= () => { ob.trs.eulerAngles = Vector3.forward * rot.val; };
		}
	}
}