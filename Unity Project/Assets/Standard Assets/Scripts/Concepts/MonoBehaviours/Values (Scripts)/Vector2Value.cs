using UnityEngine;

namespace EternityEngine
{
	public class Vector2Value : Value<Vector2>
	{
		public FloatValue xValue;
		public FloatValue yValue;

		void Awake ()
		{
			xValue.onChanged += () => { val.x = xValue.val; _OnChanged (); };
			yValue.onChanged += () => { val.y = yValue.val; _OnChanged (); };
		}

		void OnDestroy ()
		{
			xValue.onChanged -= () => { val.x = xValue.val; _OnChanged (); };
			yValue.onChanged -= () => { val.y = yValue.val; _OnChanged (); };
		}
	}
}