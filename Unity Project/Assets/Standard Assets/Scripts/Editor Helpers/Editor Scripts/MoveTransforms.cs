#if UNITY_EDITOR
using UnityEngine;

namespace EternityEngine
{
	public class MoveTransforms : EditorScript
	{
		public Transform[] transforms = new Transform[0];
		public Vector3 move;

		public override void Do ()
		{
			foreach (Transform trs in transforms)
				trs.position += move;
		}
	}
}
#else
namespace EternityEngine
{
	public class MoveTransforms : EditorScript
	{
	}
}
#endif