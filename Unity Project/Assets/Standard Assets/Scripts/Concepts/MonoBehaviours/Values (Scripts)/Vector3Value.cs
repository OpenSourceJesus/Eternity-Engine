using System;
using UnityEngine;

namespace EternityEngine
{
	public class Vector3Value : Value<Vector3>
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
		public FloatValue zValue;

		public override void Awake ()
		{
			xValue.onChanged += () => { val.x = xValue.val; _OnChanged (); };
			yValue.onChanged += () => { val.y = yValue.val; _OnChanged (); };
			zValue.onChanged += () => { val.z = zValue.val; _OnChanged (); };
		}

		public override void OnDestroy ()
		{
			xValue.onChanged -= () => { val.x = xValue.val; _OnChanged (); };
			yValue.onChanged -= () => { val.y = yValue.val; _OnChanged (); };
			zValue.onChanged -= () => { val.z = zValue.val; _OnChanged (); };
		}

		public void SetSubValues ()
		{
			xValue.val = val.x;
			yValue.val = val.y;
			zValue.val = val.z;
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
			_Data.val = _Vector3.FromVec3(val);
		}

		void SetValFromData ()
		{
			val = _Data.val.ToVec3();
		}

		[Serializable]
		public class Data : Value<Vector3>.Data
		{
			public _Vector3 val;

			public override object GenAsset ()
			{
				Vector3Value vector3Value = Instantiate(EternityEngine.instance.vector3ValuePrefab);
				Apply (vector3Value);
				return vector3Value;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				Vector3Value vector3Value = (Vector3Value) asset;
				vector3Value.SetValFromData ();
			}
		}
	}
}