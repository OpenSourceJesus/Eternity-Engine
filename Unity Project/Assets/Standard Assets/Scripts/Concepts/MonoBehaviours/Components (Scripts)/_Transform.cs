using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Transform : _Component
	{
		[HideInInspector]
		public Transform trs;
        public Vector3Value pos;
        public FloatValue rot;
        public Vector2Value size;

		public override void Start ()
		{
			pos.onChanged += OnPosChanged;
			rot.onChanged += OnRotChanged;
			size.onChanged += OnSizeChanged;
		}

		void OnDestroy ()
		{
			pos.onChanged -= OnPosChanged;
			rot.onChanged -= OnRotChanged;
			size.onChanged -= OnSizeChanged;
		}

		void OnPosChanged ()
		{
			for (int i = 0; i < ob.imgs.Length; i ++)
			{
				Image img = ob.imgs[i];
				img.rectTransform.position = pos.val;
			}
		}

		void OnRotChanged ()
		{
			for (int i = 0; i < ob.imgs.Length; i ++)
			{
				Image img = ob.imgs[i];
				img.rectTransform.eulerAngles = Vector3.forward * rot.val;
			}
		}

		void OnSizeChanged ()
		{
			for (int i = 0; i < ob.imgs.Length; i ++)
			{
				Image img = ob.imgs[i];
				img.rectTransform.localScale = size.val;
			}
		}
	}
}