/*
	This file defines a Range of bytes
*/

using System;
using Extensions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ByteRange : Range<byte>
{
	public ByteRange ()
	{
	}

	public ByteRange (byte min, byte max) : base (min, max)
	{
	}

	public bool DoesIntersect (ByteRange byteRange, bool equalIntsIntersect = true)
	{
		if (equalIntsIntersect)
			return (min >= byteRange.min && min <= byteRange.max) || (byteRange.min >= min && byteRange.min <= max) || (max <= byteRange.max && max >= byteRange.min) || (byteRange.max <= max && byteRange.max >= min);
		else
			return (min > byteRange.min && min < byteRange.max) || (byteRange.min > min && byteRange.min < max) || (max < byteRange.max && max > byteRange.min) || (byteRange.max < max && byteRange.max > min);
	}

	public bool GetIntersectionRange (ByteRange byteRange, out ByteRange? intersectionRange, bool equalIntsIntersect = true)
	{
		intersectionRange = null;
		if (DoesIntersect(byteRange, equalIntsIntersect))
			intersectionRange = new ByteRange((byte) Mathf.Max(min, byteRange.min), (byte) Mathf.Min(max, byteRange.max));
		return intersectionRange != null;
	}

	public override byte Get (float normalizedValue)
	{
		return (byte) Mathf.RoundToInt((max - min) * normalizedValue + min);
	}

	public override bool Contains (byte value, bool includeMinAndMax = true)
	{
		if (includeMinAndMax)
			return value >= min && value <= max;
		else
			return value > min && value < max;
	}
}