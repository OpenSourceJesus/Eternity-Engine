using UnityEngine;

namespace EternityEngine
{
	public class Vector3Value : Value<Vector3>
	{
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

		public void SetSubValues (Vector3 val)
		{
			xValue.val = val.x;
			xValue.HandleChange ();
			yValue.val = val.y;
			yValue.HandleChange ();
			zValue.val = val.y;
			zValue.HandleChange ();
		}
	}
}