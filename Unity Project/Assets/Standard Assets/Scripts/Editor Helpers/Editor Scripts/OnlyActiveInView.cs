#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Frogger
{
	[ExecuteInEditMode]
	public class OnlyActiveInView : EditorScript
	{
		public Transform trs;
		public static List<OnlyActiveInView> instances = new List<OnlyActiveInView>();

		void Awake ()
		{
			instances.Add(this);
			if (trs == null)
				trs = transform;
		}

		void OnDestroy ()
		{
			instances.Remove(this);
		}
	}
}
#else
namespace Frogger
{
	public class OnlyActiveInView : EditorScript
	{
	}
}
#endif