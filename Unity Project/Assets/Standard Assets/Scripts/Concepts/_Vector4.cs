using System;
using UnityEngine;

[Serializable]
public struct _Vector4
{
	public float x;
	public float y;
	public float z;
	public float w;

	public _Vector4 (float x, float y, float z, float w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	public Vector4 ToVec4 ()
	{
		return new Vector4(x, y, z, w);
	}

	public Color ToColor ()
	{
		return new Color(x, y, z, w);
	}

	public static _Vector4 FromVec4 (Vector4 v)
	{
		return new _Vector4(v.x, v.y, v.z, v.w);
	}

	public static _Vector4 FromColor (Color c)
	{
		return new _Vector4(c.r, c.g, c.b, c.a);
	}
}