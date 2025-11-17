using UnityEngine;

namespace EternityEngine
{
	public class ColorValue : Value<Color>
	{
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
	}
}