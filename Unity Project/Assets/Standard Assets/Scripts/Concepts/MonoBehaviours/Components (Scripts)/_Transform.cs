using System;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Transform : _Component
	{
		public new Data _Data
		{
			get
			{
				if (data == null)
					data = new Data();
				return (Data) data;
			}
			set
			{
				data = value;
			}
		}
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
			ob.trs.position = pos.val;
			for (int i = 0; i < ob.sceneEntries.Length; i ++)
			{
				SceneEntry sceneEntry = ob.sceneEntries[i];
				sceneEntry.rectTrs.position = pos.val;
			}
		}

		void OnRotChanged ()
		{
			ob.trs.eulerAngles = Vector3.forward * rot.val;
			for (int i = 0; i < ob.sceneEntries.Length; i ++)
			{
				SceneEntry sceneEntry = ob.sceneEntries[i];
				sceneEntry.rectTrs.eulerAngles = Vector3.forward * rot.val;
			}
		}

		void OnSizeChanged ()
		{
			ob.trs.localScale = size.val;
			for (int i = 0; i < ob.sceneEntries.Length; i ++)
			{
				SceneEntry sceneEntry = ob.sceneEntries[i];
				sceneEntry.rectTrs.localScale = size.val;
			}
		}

		[Serializable]
		public class Data : _Component.Data
		{
			public _Vector3 pos;
			public float rot;
			public _Vector2 size;

			public override void Set (_Component component)
			{
				base.Set (component);
				_Transform trs = (_Transform) component;
				pos = _Vector3.FromVec3(trs.pos.val);
				rot = trs.rot.val;
				size = _Vector2.FromVec2(trs.size.val);
			}

			public override void Apply (_Component component)
			{
				base.Apply (component);
				_Transform trs = (_Transform) component;
				trs.pos.Set (pos.ToVec3());
				trs.rot.Set (rot);
				trs.size.Set (size.ToVec2());
			}
		}
	}
}