#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Frogger
{
	public class SetAllObjectsDirty : EditorScript
	{
		public override void Do ()
		{
			_Do ();
		}

		[MenuItem("Tools/Set all Objects dirty")]
		static void _Do ()
		{
			Object[] objects = FindObjectsOfType<Object>();
			for (int i = 0; i < objects.Length; i ++)
			{
				Object obj = objects[i];
				EditorUtility.SetDirty(obj);
			}
		}
	}
}
#else
namespace Frogger
{
	public class SetAllObjectsDirty : EditorScript
	{
	}
}
#endif