using UnityEngine;

namespace EternityEngine
{
	public class Vector2Value : Value<Vector2>
	{
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

		public void SetSubValues (Vector2 val)
		{
			xValue.val = val.x;
			xValue.HandleChange ();
			yValue.val = val.y;
			yValue.HandleChange ();
		}
	}
}