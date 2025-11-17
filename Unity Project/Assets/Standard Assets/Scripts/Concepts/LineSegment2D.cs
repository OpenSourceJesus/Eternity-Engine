using System;
using Extensions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class LineSegment2D
{
	public Vector2 start;
	public Vector2 end;

	public LineSegment2D ()
	{
	}

	public LineSegment2D (Vector2 start, Vector2 end)
	{
		this.start = start;
		this.end = end;
	}

#if UNITY_EDITOR
	public virtual void DrawGizmos (Color color)
	{
		GizmosManager.GizmosEntry gizmosEntry = new GizmosManager.GizmosEntry();
		gizmosEntry.setColor = true;
		gizmosEntry.color = color;
		gizmosEntry.onDrawGizmos += DrawGizmos;
		gizmosEntry.remove = true;
		GizmosManager.gizmosEntries.Add(gizmosEntry);
		Debug.DrawLine(start, end, color, .1f);
	}

	public virtual void DrawGizmos (object arg)
	{
		Gizmos.DrawLine(start, end);
	}
#endif

	public override string ToString ()
	{
		return "[" + start + "], [" + end + "]";
	}
	
	public float GetSlope ()
	{
		return (end.y - start.y) / (end.x - start.x);
	}
	
	public float GetFacingAngle ()
	{
		return (end - start).GetFacingAngle();
	}

	public bool DoIIntersect (LineSegment2D other, bool shouldIncludeEndPoints)
	{
		bool output = false;
		float denominator = (other.end.y - other.start.y) * (end.x - start.x) - (other.end.x - other.start.x) * (end.y - start.y);
		if (denominator != 0f)
		{
			float u_a = ((other.end.x - other.start.x) * (start.y - other.start.y) - (other.end.y - other.start.y) * (start.x - other.start.x)) / denominator;
			float u_b = ((end.x - start.x) * (start.y - other.start.y) - (end.y - start.y) * (start.x - other.start.x)) / denominator;
			if (shouldIncludeEndPoints)
			{
				if (u_a >= 0f && u_a <= 1f && u_b >= 0f && u_b <= 1f)
					output = true;
			}
			else
			{
				if (u_a > 0f && u_a < 1f && u_b > 0f && u_b < 1f)
					output = true;
			}
		}
		return output;
	}

	// public bool DoIIntersect (Vector2 center, float radius)
	// {
	// 	return Vector2.Distance(ClosestPoint(center), center) <= radius;
	// }

	// public bool DoIIntersect (Vector2 center, float radius)
	// {
	// 	return Vector2.Distance(GetPointWithDirectedDistance(GetDirectedDistanceAlongParallel(center)), center) <= radius;
	// }

	public bool DoIIntersect (Vector2 center, float radius)
	{
		Vector2 lineDirection = GetDirection();
		Vector2 centerToLineStart = start - center;
		float a = Vector2.Dot(lineDirection, lineDirection);
		float b = 2 * Vector2.Dot(centerToLineStart, lineDirection);
		float c = Vector2.Dot(centerToLineStart, centerToLineStart) - radius * radius;
		float discriminant = b * b - 4 * a * c;
		if (discriminant >= 0)
		{
			discriminant = Mathf.Sqrt(discriminant);
			float t1 = (-b - discriminant) / (2 * a);
			float t2 = (-b + discriminant) / (2 * a);
			if (t1 >= 0 && t1 <= 1 || t2 >= 0 && t2 <= 1)
				return true;
		}
		return false;
	}
	
	public bool Contains (Vector2 point)
	{
		return Vector2.Distance(point, start) + Vector2.Distance(point, end) == Vector2.Distance(start, end);
	}

	public bool Encapsulates (LineSegment2D lineSegment)
	{
		return Contains(lineSegment.start) && Contains(lineSegment.end);
	}
	
	public LineSegment2D Move (Vector2 movement)
	{
		return new LineSegment2D(start + movement, end + movement);
	}
	
	public LineSegment2D Rotate (Vector2 pivotPoint, float degrees)
	{
		LineSegment2D output;
		Vector2 outputStart = start.Rotate(pivotPoint, degrees);
		Vector2 outputEnd = end.Rotate(pivotPoint, degrees);
		output = new LineSegment2D(outputStart, outputEnd);
		return output;
	}

	public Vector2 GetClosestPoint (Vector2 point)
	{
		Vector2 output;
		float directedDistanceAlongParallel = GetDirectedDistanceAlongParallel(point);
		if (directedDistanceAlongParallel > 0 && directedDistanceAlongParallel < GetLength())
			output = GetPointWithDirectedDistance(directedDistanceAlongParallel);
		else if (directedDistanceAlongParallel >= GetLength())
			output = end;
		else
			output = start;
		return output;
	}
	
	public float GetDistanceSqrTo (Vector2 point)
	{
		return (point - GetClosestPoint(point)).sqrMagnitude;
	}

	public float GetDistanceTo (Vector2 point)
	{
		return (point - GetClosestPoint(point)).magnitude;
	}

	public LineSegment2D GetPerpendicular (bool rotateClockwise = false)
	{
		if (rotateClockwise)
			return Rotate(GetMidpoint(), -90);
		else
			return Rotate(GetMidpoint(), 90);
	}

	public Vector2 GetMidpoint ()
	{
		return (start + end) / 2;
	}

	public float GetDirectedDistanceAlongParallel (Vector2 point)
	{
		float rotate = -GetFacingAngle();
		LineSegment2D rotatedLine = Rotate(Vector2.zero, rotate);
		point = point.Rotate(rotate);
		return point.x - rotatedLine.start.x;
	}

	public Vector2 GetPointWithDirectedDistance (float directedDistance)
	{
		return start + (GetDirection() * directedDistance);
	}

	public float GetLength ()
	{
		return Vector2.Distance(start, end);
	}

	public Vector2 GetDirection ()
	{
		return (end - start).normalized;
	}
	
	public LineSegment2D Flip ()
	{
		return new LineSegment2D(end, start);
	}

	public bool GetIntersection (LineSegment2D lineSegment, out Vector2 intersection, bool collinearOverlapsIntersect = true)
	{
		intersection = new Vector2();
		Vector2 r = end - start;
		Vector2 s = lineSegment.end - lineSegment.start;
		float rxs = r.Cross(s);
		float qpxr = (lineSegment.start - start).Cross(r);
		if (Mathf.Approximately(rxs, 0) && Mathf.Approximately(qpxr, 0))
			return collinearOverlapsIntersect && ((0 <= (lineSegment.start - start).Multiply_float(r) && (lineSegment.start - start).Multiply_float(r) <= r.Multiply_float(r)) || (0 <= (start - lineSegment.start).Multiply_float(s) && (start - lineSegment.start).Multiply_float(s) <= s.Multiply_float(s)));
		if (Mathf.Approximately(rxs, 0) && !Mathf.Approximately(qpxr, 0))
			return false;
		float t = (lineSegment.start - start).Cross(s) / rxs;
		float u = (lineSegment.start - start).Cross(r) / rxs;
		if (!Mathf.Approximately(rxs, 0) && (0 <= t && t <= 1) && (0 <= u && u <= 1))
		{
			intersection = start + (t * r);
			return true;
		}
		return false;
	}
	
	public float GetYOfPoint (float x)
	{
		if (start.x == end.x)
			throw new Exception("The x-component of the start and end points of the line segment are both " + end.x);
		else
			return GetSlope() * x + GetYIntercept();
	}
	
	public float GetXOfPoint (float y)
	{
		if (start.y == end.y)
			throw new Exception("The y-component of the start and end points of the line segment are both " + end.y);
		else
			return (-GetYIntercept() + y) / GetSlope();
	}

	public float GetXIntercept ()
	{
		float a = start.y - end.y;
		float b = start.x - end.x;
		if (b == 0)
			return start.x;
		else if (a == 0)
			return Mathf.Infinity;
		else
		{
			float slope = GetSlope();
			float c = start.y - slope * start.x;
			return -c / slope;
		}
	}

	public float GetYIntercept ()
	{
		float a = start.y - end.y;
		float b = start.x - end.x;
		if (b == 0)
			return Mathf.Infinity;
		else if (a == 0)
			return start.y;
		else
		{
			float slope = GetSlope();
			float c = start.y - slope * start.x;
			return c;
		}
	}

	public float GetDistanceSqr (LineSegment2D lineSegment, float checkDistanceInterval = .1f)
	{
		float output = Mathf.Infinity;
		for (float distance = 0; distance <= GetLength(); distance += checkDistanceInterval)
		{
			Vector2 point = GetPointWithDirectedDistance(distance);
			for (float distance2 = 0; distance2 <= lineSegment.GetLength(); distance2 += checkDistanceInterval)
			{
				Vector2 point2 = lineSegment.GetPointWithDirectedDistance(distance2);
				output = Mathf.Min((point - point2).sqrMagnitude, output);
			}
		}
		return output;
	}

	public float GetDistance (LineSegment2D lineSegment, float checkDistanceInterval = .1f)
	{
		return Mathf.Sqrt(GetDistanceSqr(lineSegment, checkDistanceInterval));
	}

	public (Vector2 point, Vector2 point2) GetClosestPointsTo (LineSegment2D lineSegment)
	{
		throw new NotImplementedException();
		// Vector2 P1 = start;
		// Vector2 P2 = lineSegment.start;
		// Vector2 V1 = end - start;
		// Vector2 V2 = lineSegment.end - lineSegment.start;
		// Vector2 V21 = P2 - P1;
		// float v22 = Vector2.Dot(V2, V2);
		// float v11 = Vector2.Dot(V1, V1);
		// float v21 = Vector2.Dot(V2, V1);
		// float v21_1 = Vector2.Dot(V21, V1);
		// float v21_2 = Vector2.Dot(V21, V2);
		// float denom = v21 * v21 - v22 * v11;
		// float s;
		// float t;
		// if (Mathf.Approximately(denom, 0))
		// {
		// 	s = 0;
		// 	t = (v11 * s - v21_1) / v21;
		// }
		// else
		// {
		// 	s = (v21_2 * v21 - v22 * v21_1) / denom;
		// 	t = (-v21_1 * v21 + v11 * v21_2) / denom;
		// }
		// s = Mathf.Max(Mathf.Min(s, 1), 0);
		// t = Mathf.Max(Mathf.Min(t, 1), 0);
		// Vector2 p_a = P1 + (V1 * s);
		// Vector2 p_b = P2 + (V2 * t);
		// return (p_a, p_b);
	}

	// public LineSegment2D[] GetErasedLineSegment (LineSegment2D eraser, Vector2 movement)
	// {
	// 	LineSegment2D[] output = new LineSegment2D[1] { this };
	// 	LineSegment2D eraserStartMovementLine = new LineSegment2D(eraser.start, eraser.start + movement);
	// 	LineSegment2D eraserEndMovementLine = new LineSegment2D(eraser.end, eraser.end + movement);
	// 	Vector2 eraserStartIntersection;
	// 	Vector2 eraserEndIntersection;
	// 	Vector2 eraserStartMovementIntersection;
	// 	Vector2 eraserEndMovementIntersection;
	// 	bool eraserStartIntersects = GetIntersectionWithLineSegment(eraser.start, out eraserStartIntersection);
	// 	bool eraserEndIntersects = GetIntersectionWithLineSegment(eraser.end, out eraserEndIntersection);
	// 	bool eraserStartMovementIntersects = GetIntersectionWithLineSegment(eraserStartMovementLine, out eraserStartMovementIntersection);
	// 	bool eraserEndMovementIntersects = GetIntersectionWithLineSegment(eraserEndMovementLine, out eraserEndMovementIntersection);
	// 	int intersectionCount = 0;
	// 	if (eraserStartIntersects)
	// 		intersectionCount ++;
	// 	if (eraserEndIntersects)
	// 		intersectionCount ++;
	// 	if (eraserStartMovementIntersects)
	// 		intersectionCount ++;
	// 	if (eraserEndMovementIntersects)
	// 		intersectionCount ++;
	// 	if (intersectionCount == 2)
	// 	{
			
	// 	}
	// 	else if (intersectionCount == 1)
	// 	{

	// 	}
	// 	else
	// 	{
			
	// 	}
	// 	return output;
	// }
}