using System;
using UnityEngine;

namespace EternityEngine
{
	public class Vector2Value : Value<Vector2>
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
		public FloatValue xValue;
		public FloatValue yValue;

		public override void Awake ()
		{
			xValue.onChanged += () => { val.x = xValue.val; _OnChanged (); };
			yValue.onChanged += () => { val.y = yValue.val; _OnChanged (); };
		}

		public override void OnDestroy ()
		{
			xValue.onChanged -= () => { val.x = xValue.val; _OnChanged (); };
			yValue.onChanged -= () => { val.y = yValue.val; _OnChanged (); };
		}

		public void SetSubValues ()
		{
			xValue.val = val.x;
			yValue.val = val.y;
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
			SetValOfData ();
		}

		void SetValOfData ()
		{
			_Data.val = _Vector2.FromVec2(val);
		}

		void SetValFromData ()
		{
			val = _Data.val.ToVec2();
		}

		[Serializable]
		public class Data : Value<Vector2>.Data
		{
			public _Vector2 val;

			public override object GenAsset ()
			{
				Vector2Value vector2Value = Instantiate(EternityEngine.instance.vector2ValuePrefab);
				Apply (vector2Value);
				return vector2Value;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				Vector2Value vector2Value = (Vector2Value) asset;
				vector2Value.SetValFromData ();
			}
		}
	}
}