using System;
using UnityEngine;

namespace EternityEngine
{
	public class ColorValue : Value<Color>
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
		public FloatValue rValue;
		public FloatValue gValue;
		public FloatValue bValue;
		public FloatValue aValue;

		public override void Awake ()
		{
			rValue.onChanged += () => { val.r = rValue.val; _OnChanged (); };
			gValue.onChanged += () => { val.g = gValue.val; _OnChanged (); };
			bValue.onChanged += () => { val.b = bValue.val; _OnChanged (); };
			aValue.onChanged += () => { val.a = aValue.val; _OnChanged (); };
		}

		public override void OnDestroy ()
		{
			rValue.onChanged -= () => { val.r = rValue.val; _OnChanged (); };
			gValue.onChanged -= () => { val.g = gValue.val; _OnChanged (); };
			bValue.onChanged -= () => { val.b = bValue.val; _OnChanged (); };
			aValue.onChanged -= () => { val.a = aValue.val; _OnChanged (); };
		}

		public void SetSubValues ()
		{
			rValue.val = val.r;
			gValue.val = val.g;
			bValue.val = val.b;
			aValue.val = val.a;
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
			_Data.val = _Vector4.FromColor(val);
		}

		void SetValFromData ()
		{
			val = _Data.val.ToColor();
		}

		[Serializable]
		public class Data : Value<Color>.Data
		{
			public _Vector4 val;

			public override object GenAsset ()
			{
				ColorValue colorValue = Instantiate(EternityEngine.instance.colorValuePrefab);
				Apply (colorValue);
				return colorValue;
			}

			public override void Apply (Asset asset)
			{
				asset.data = SaveAndLoadManager.saveData.assetsDatasDict[id];
				base.Apply (asset);
				ColorValue colorValue = (ColorValue) asset;
				colorValue.SetValFromData ();
			}
		}
	}
}