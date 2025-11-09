#if UNITY_EDITOR
using Extensions;
using UnityEngine;

namespace EternityEngine
{
	public class SetLineRendererUseWorldSpace : EditorScript
	{
		public LineRenderer lineRenderer;
		public bool useWorldSpace;

		public override void Do ()
		{
			if (lineRenderer == null)
				lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.SetUseWorldSpace (lineRenderer.transform, useWorldSpace);
		}
	}
}
#else
namespace EternityEngine
{
	public class SetLineRendererUseWorldSpace : EditorScript
	{
	}
}
#endif