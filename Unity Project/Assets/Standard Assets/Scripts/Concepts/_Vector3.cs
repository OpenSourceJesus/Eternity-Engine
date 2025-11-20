using System;
using UnityEngine;

[Serializable]
public struct _Vector3
{
	public float x;
	public float y;
	public float z;

	public _Vector3 (float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3 ToVec3 ()
	{
		return new Vector3(x, y, z);
	}

	public static _Vector3 FromVec3 (Vector3 v)
	{
		return new _Vector3(v.x, v.y, v.z);
	}
}