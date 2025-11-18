using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Transform : _Component
	{
        public Vector3Value pos;
        public FloatValue rot;
        public Vector2Value size;

		public override void Init ()
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
			for (int i = 0; i < ob.sceneEntries.Length; i ++)
			{
				SceneEntry sceneEntry = ob.sceneEntries[i];
				sceneEntry.rectTrs.position = pos.val;
			}
		}

		void OnRotChanged ()
		{
			for (int i = 0; i < ob.sceneEntries.Length; i ++)
			{
				SceneEntry sceneEntry = ob.sceneEntries[i];
				sceneEntry.rectTrs.eulerAngles = Vector3.forward * rot.val;
			}
		}

		void OnSizeChanged ()
		{
			for (int i = 0; i < ob.sceneEntries.Length; i ++)
			{
				SceneEntry sceneEntry = ob.sceneEntries[i];
				sceneEntry.rectTrs.localScale = size.val;
			}
		}
	}
}