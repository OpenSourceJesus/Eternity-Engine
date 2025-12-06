using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace EternityEngine
{
	public class AttributeValue<T> : Value<Dictionary<string, T>>
	{
		public override void Awake ()
		{
			base.Awake ();
			val = new Dictionary<string, T>();
		}
	}
}