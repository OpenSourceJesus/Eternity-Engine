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

		public override void InitData ()
		{
			if (data == null)
				data = new Data();
		}

		public override void SetData ()
		{
			InitData ();
			base.SetData ();
			SetPosOfData ();
			SetRotOfData ();
			SetSizeOfData ();
		}

		void SetPosOfData ()
		{
			_Data.pos = _Vector3.FromVec3(pos.val);
		}

		void SetPosFromData ()
		{
			pos.Set (_Data.pos.ToVec3());
		}

		void SetRotOfData ()
		{
			_Data.rot = rot.val;
		}

		void SetRotFromData ()
		{
			rot.Set (_Data.rot);
		}

		void SetSizeOfData ()
		{
			_Data.size = _Vector2.FromVec2(size.val);
		}

		void SetSizeFromData ()
		{
			size.Set (_Data.size.ToVec2());
		}

		[Serializable]
		public class Data : _Component.Data
		{
			public _Vector3 pos;
			public float rot;
			public _Vector2 size;

			public override object GenAsset ()
			{
				_Transform trs = Instantiate(EternityEngine.instance.trsPrefab);
				Apply (trs);
				return trs;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				_Transform trs = (_Transform) asset;
				trs.SetPosFromData ();
				trs.SetRotFromData ();
				trs.SetSizeFromData ();
			}
		}
	}
}