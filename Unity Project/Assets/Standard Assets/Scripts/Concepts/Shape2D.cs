using System;
using Extensions;
using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public class Shape2D
{
	public Vector2[] corners = new Vector2[0];
	public LineSegment2D[] edges = new LineSegment2D[0];

	public Shape2D ()
	{
	}

	public Shape2D (Shape2D shape)
	{
		corners = new Vector2[shape.corners.Length];
		shape.corners.CopyTo(corners, 0);
		edges = new LineSegment2D[shape.edges.Length];
		shape.edges.CopyTo(edges, 0);
	}

#if UNITY_EDITOR
	public void DrawGizmos (Color color)
	{
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			edge.DrawGizmos (color);
		}
	}
#endif

	public void SetCorners_Polygon ()
	{
		corners = new Vector2[edges.Length];
		for (int i = 0; i < edges.Length; i ++)
			corners[i] = edges[i].end;
	}

	public void SetEdges_Polygon ()
	{
		edges = new LineSegment2D[corners.Length];
		Vector3 previousCorner = corners[corners.Length - 1];
		for (int i = 0; i < corners.Length; i ++)
		{
			Vector2 corner = corners[i];
			edges[i] = new LineSegment2D(previousCorner, corner);
			previousCorner = corner;
		}
	}

	public float GetPerimeter ()
	{
		float output = 0;
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			output += edge.GetLength();
		}
		return output;
	}

	public Vector2 GetPointOnPerimeter (float distance)
	{
		float perimeter = GetPerimeter();
		while (true)
		{
			for (int i = 0; i < edges.Length; i ++)
			{
				LineSegment2D edge = edges[i];
				float edgeLength = edge.GetLength();
				distance -= edgeLength;
				if (distance <= 0)
					return edge.GetPointWithDirectedDistance(edgeLength + distance);
			}
		}
	}

	public bool Contains_Polygon (Vector2 point, bool equalPointsIntersect = true, float checkDistance = 99999)
	{
		LineSegment2D checkLineSegment = new LineSegment2D(point, point + (Random.insideUnitCircle.normalized * checkDistance));
		int collisionCount = 0;
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			if (edge.DoIIntersect(checkLineSegment, equalPointsIntersect))
				collisionCount ++;
		}
		return collisionCount % 2 == 1;
	}

	public bool CrossesOverSelf ()
	{
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			for (int i2 = i + 1; i2 < edges.Length; i2 ++)
			{
				LineSegment2D edge2 = edges[i2];
				if (edge.DoIIntersect(edge2, false))
					return true;
			}
		}
		return false;
	}

	public bool DoIIntersect (LineSegment2D lineSegment, bool equalPointsIntersect = true)
	{
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			if (edge.DoIIntersect(lineSegment, equalPointsIntersect))
				return true;
		}
		return false;
	}

	public Vector2 GetRandomPoint (bool checkIfContained = true, bool containsEdges = true, float checkDistance = 99999)
	{
		float perimeter = GetPerimeter();
		while (true)
		{
			Vector2 point1 = GetPointOnPerimeter(Random.Range(0, perimeter));
			Vector2 point2 = GetPointOnPerimeter(Random.Range(0, perimeter));
			Vector2 output = (point1 + point2) / 2;
			if (!checkIfContained || Contains_Polygon(output, containsEdges, checkDistance))
				return output;
		}
	}

	public Vector2 GetClosestPoint (Vector2 point, bool onlyCheckEdges = true, float checkDistance = 99999)
	{
		(Vector2 point, float distanceSqr) closestPointAndDistanceSqr = GetClosestPointAndDistanceSqr(point, onlyCheckEdges, checkDistance);
		return closestPointAndDistanceSqr.point;
	}

	public float GetDistanceSqr (Vector2 point, bool onlyCheckEdges = true, float checkDistance = 99999)
	{
		(Vector2 point, float distanceSqr) closestPointAndDistanceSqr = GetClosestPointAndDistanceSqr(point, onlyCheckEdges, checkDistance);
		return closestPointAndDistanceSqr.distanceSqr;
	}

	public (Vector2, float) GetClosestPointAndDistanceSqr (Vector2 point, bool onlyCheckEdges = true, float checkDistance = 99999)
	{
		if (!onlyCheckEdges && Contains_Polygon(point, checkDistance: checkDistance))
			return (point, 0);
		else
		{
			Vector2 closestPoint = new Vector2();
			float closestDistanceSqr = Mathf.Infinity;
			for (int i = 0; i < edges.Length; i ++)
			{
				LineSegment2D edge = edges[i];
				Vector2 pointOnPerimeter = edge.GetClosestPoint(point);
				float distanceSqr = (point - pointOnPerimeter).sqrMagnitude;
				if (distanceSqr < closestDistanceSqr)
				{
					closestDistanceSqr = distanceSqr;
					closestPoint = pointOnPerimeter;
				}
			}
			return (closestPoint, closestDistanceSqr);
		}
	}

	public bool DoIIntersectPolygon (Shape2D shape, bool equalPointsIntersect = true, float checkDistance = 99999)
	{
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			if (shape.DoIIntersect(edge, equalPointsIntersect))
				return true;
		}
		return Contains_Polygon(corners[0], equalPointsIntersect, checkDistance);
	}

	public Shape2D Subdivide ()
	{
		List<LineSegment2D> output = new List<LineSegment2D>();
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			Vector2 midPoint = edge.GetMidpoint();
			output.Add(new LineSegment2D(edge.start, midPoint));
			output.Add(new LineSegment2D(midPoint, edge.end));
		}
		return FromEdgesWithLoop(output.ToArray());
	}

	public Shape2D Combine (Shape2D shape)
	{
		throw new NotImplementedException();
	}

	public List<Vector2> GetIntersections (Shape2D shape, bool cornersCanIntersect = false)
	{
		List<Vector2> output = new List<Vector2>();
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			for (int i2 = 0; i2 < shape.edges.Length; i2 ++)
			{
				LineSegment2D edge2 = shape.edges[i2];
				Vector2 intersectionPoint;
				if (edge.GetIntersection(edge2, out intersectionPoint, true) && (cornersCanIntersect || !corners.Contains(intersectionPoint)))
					output.Add(intersectionPoint);
			}
		}
		return output;
	}

	public Shape2D IntersectionOfConvexPolygonForConvexPolygon (Shape2D shape, float checkDistance = 99999)
	{
		List<Vector2> outputCorners = GetIntersections(shape);
		if (outputCorners.Count == 0)
		{
			if (Contains_Polygon(shape.corners[0], true, checkDistance))
				return FromEdgesWithLoop(shape.edges);
			else if (shape.Contains_Polygon(this.corners[0], true, checkDistance))
				return FromEdgesWithLoop(edges);
		}
		else
		{
			for (int i = 0; i < this.corners.Length; i ++)
			{
				Vector2 corner = this.corners[i];
				if (shape.Contains_Polygon(corner, true, checkDistance) && !outputCorners.Contains(corner))
					outputCorners.Add(corner);
			}
			for (int i = 0; i < shape.corners.Length; i ++)
			{
				Vector2 corner = shape.corners[i];
				if (Contains_Polygon(corner, true, checkDistance) && !outputCorners.Contains(corner))
					outputCorners.Add(corner);
			}
			List<List<Vector2>> uniquePermutations = outputCorners.ToArray().UniquePermutations();
			for (int i = 0; i < uniquePermutations.Count; i ++)
			{
				List<Vector2> uniquePermutation = uniquePermutations[i];
				Shape2D output = FromCornersWithLoop(uniquePermutation.ToArray());
				if (output.ContainsTheSameEdgeDirectionsAsGroup(this, shape))
					return output;
			}
		}
		return null;
	}

	public Shape2D Boolean_Polygon (Shape2D toggle)
	{
		throw new NotImplementedException();
	}

	public Shape2D Remove_Polygon (Shape2D remove)
	{
		List<Vector2> corners = new List<Vector2>(this.corners);
		corners.AddRange(remove.corners);
		corners = corners.RemoveEach(IntersectionOfConvexPolygonForConvexPolygon(remove).corners);
		List<List<Vector2>> uniquePermutations = corners.ToArray().UniquePermutations();
		for (int i = 0; i < uniquePermutations.Count; i ++)
		{
			List<Vector2> uniquePermutation = uniquePermutations[i];
			Shape2D output = FromCornersWithLoop(uniquePermutation.ToArray());
			if (output.ContainsTheSameEdgeDirectionsAsGroup(this, remove))
				return output;
		}
		return null;
	}

	public Shape2D Smooth_Polygon ()
	{
		Vector2[] outputCorners = new Vector2[corners.Length];
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			outputCorners[i] = edge.GetMidpoint();
		}
		return FromCornersWithLoop(outputCorners);
	}

	public Shape2D GrowFrom_Polygon (float amount, Vector2 point)
	{
		Vector2[] outputCorners = new Vector2[corners.Length];
		for (int i = 0; i < corners.Length; i ++)
		{
			Vector2 corner = corners[i];
			Vector2 fromPoint = corner - point;
			outputCorners[i] = corner + fromPoint.normalized * (fromPoint.magnitude + amount);
		}
		return FromCornersWithLoop(outputCorners);
	}

	public Shape2D ShrinkTo_Polygon (float amount, Vector2 point, bool shrinkPastPoint = false)
	{
		Vector2[] outputCorners = new Vector2[corners.Length];
		for (int i = 0; i < corners.Length; i ++)
		{
			Vector2 corner = corners[i];
			Vector2 toPoint = point - corner;
			if (shrinkPastPoint)
				outputCorners[i] = corner + toPoint.normalized * (toPoint.magnitude + amount);
			else
			{
				Vector2 newCorner = corner + toPoint.normalized * (toPoint.magnitude + amount);
				if (Vector2.Dot(point - corner, point - newCorner) >= 0)
					outputCorners[i] = newCorner;
			}
		}
		return FromCornersWithLoop(outputCorners);
	}

	public Shape2D GetInterpolated (Shape2D to, float normalizedAmount)
	{
		Vector2[] outputCorners = new Vector2[corners.Length];
		for (int i = 0; i < corners.Length; i ++)
		{
			Vector2 corner = corners[i];
			Vector2 toCorner = to.corners[i];
			outputCorners[i] = Vector2.Lerp(corner, toCorner, normalizedAmount);
		}
		return FromCornersWithLoop(outputCorners);
	}

	public Shape2D Merge_Polygon (Shape2D shape)
	{
		List<Vector2> corners = new List<Vector2>(this.corners);
		corners.AddRange(shape.corners);
		List<List<Vector2>> uniquePermutations = corners.ToArray().UniquePermutations();
		for (int i = 0; i < uniquePermutations.Count; i ++)
		{
			List<Vector2> uniquePermutation = uniquePermutations[i];
			Shape2D output = FromCornersWithLoop(uniquePermutation.ToArray());
			if (output.ContainsTheSameEdgeDirectionsAsGroup(this, shape))
				return output;
		}
		return null;
	}

	public Shape2D Trim_ConvexPolygon (LineSegment2D lineSegment, bool trimClockwiseSideOfLineSegmentStart)
	{
		List<Vector2> outputCorners = new List<Vector2>(corners);
		List<Vector2> intersections = new List<Vector2>();
		LineSegment2D perpendicularLineSegment = lineSegment.GetPerpendicular(trimClockwiseSideOfLineSegmentStart);
		for (uint i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			Vector2 intersection;
			if (edge.GetIntersection(lineSegment, out intersection))
			{
				uint previousCornerIndex = i;
				uint nextCornerIndex = i + 1;
				if (i == edges.Length - 1)
					nextCornerIndex = 0;
				if (perpendicularLineSegment.GetDirectedDistanceAlongParallel(corners[nextCornerIndex]) > lineSegment.GetLength() / 2)
					outputCorners.Insert((int) nextCornerIndex + intersections.Count, intersection);
				else
					outputCorners.Insert((int) previousCornerIndex + intersections.Count, intersection);
				intersections.Add(intersection);
			}
		}
		if (intersections.Count > 0)
		{
			for (int i = 0; i < outputCorners.Count; i ++)
			{
				Vector2 outputCorner = outputCorners[i];
				if (!intersections.Contains(outputCorner) && perpendicularLineSegment.GetDirectedDistanceAlongParallel(outputCorner) > lineSegment.GetLength() / 2)
				{
					outputCorners.RemoveAt(i);
					i --;
				}
			}
		}
		return FromCornersWithLoop(outputCorners.ToArray());
	}

	public Shape2D CollapseDuplicateCorners_Polygon ()
	{
		Shape2D output = new Shape2D(this);
		List<Vector2> pastCorners = new List<Vector2>();
		for (int i = 0; i < corners.Length; i ++)
		{
			Vector2 corner = corners[i];
			if (pastCorners.Contains(corner))
				output = output.RemoveEdge_Polygon(i);
			else
				pastCorners.Add(corner);
		}
		return output;
	}

	public Shape2D RemoveEdge_Polygon (int edgeIndex)
	{
		return FromEdgesWithLoop(edges.RemoveAt(edgeIndex));
	}

	public Shape2D InsertCorner_Polygon (int cornerIndex, Vector2 corner)
	{
		return FromCornersWithLoop(corners.Insert(corner, cornerIndex));
	}

	public Shape2D InsertCornerBetweenClosestEdge_Polygon (Vector2 corner)
	{
		int closestCornerIdx = 0;
		float closestEdgeDistanceSqr = edges[0].GetDistanceSqrTo(corner);
		for (int i = 1; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			float distanceSqr = edge.GetDistanceSqrTo(corner);
			if (distanceSqr < closestEdgeDistanceSqr)
			{
				closestCornerIdx = i;
				closestEdgeDistanceSqr = distanceSqr;
			}
		}
		return InsertCorner_Polygon(closestCornerIdx, corner);
	}

	public bool ContainsTheSameEdgeDirectionsAsGroup (params Shape2D[] shapes)
	{
		List<Vector2> correctDirections = new List<Vector2>();
		for (int i = 0; i < shapes.Length; i ++)
		{
			Shape2D shape = shapes[i];
			for (int i2 = 0; i2 < shape.edges.Length; i2 ++)
			{
				LineSegment2D edge = shape.edges[i2];
				correctDirections.Add(edge.GetDirection());
			}
		}
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			if (!correctDirections.Contains(edge.GetDirection()))
				return false;
		}
		return true;
	}

	public Vector2 GetCornerAverage ()
	{
		Vector2 output = new Vector2();
		for (int i = 0; i < corners.Length; i ++)
		{
			Vector2 corner = corners[i];
			output += corner;
		}
		return output / corners.Length;
	}

	public int GetClosestCornerIndex (Vector2 point, bool checkEdges = false)
	{
		if (!checkEdges)
			return VectorExtensions.GetIndexOfClosestPoint(point, corners);
		else
		{
			int closestCornerIdx = 0;
			float closestDistanceSqr = Mathf.Infinity;
			for (int i = 0; i < edges.Length; i ++)
			{
				LineSegment2D edge = edges[i];
				Vector2 pointOnPerimeter = edge.GetClosestPoint(point);
				float distanceSqr = (point - pointOnPerimeter).sqrMagnitude;
				if (distanceSqr < closestDistanceSqr)
				{
					closestDistanceSqr = distanceSqr;
					if (i == 0)
						closestCornerIdx = edges.Length - 1;
					else
						closestCornerIdx = i - 1;
				}
			}
			return closestCornerIdx;
		}
	}

	public float GetDistanceSqr (Shape2D shape)
	{
		float output = Mathf.Infinity;
		for (int i = 0; i < edges.Length; i ++)
		{
			LineSegment2D edge = edges[i];
			for (int i2 = 0; i2 < shape.edges.Length; i2 ++)
			{
				LineSegment2D edge2 = shape.edges[i2];
				output = Mathf.Min(edge.GetDistanceSqr(edge2), output);
			}
		}
		return output;
	}

#if SPLINES
	public void ToSpline (Spline spline)
	{
		for (int i = 0; i < corners.Length; i ++)
		{
			Vector2 corner = corners[i];
			if (i >= spline.GetPointCount())
				spline.InsertPointAt(spline.GetPointCount(), corner);
			else
				spline.SetPosition(i, corner);
		}
		for (int i = corners.Length; i < spline.GetPointCount(); i ++)
		{
			spline.RemovePointAt(i);
			i --;
		}
	}

	public static Shape2D FromSpline (Spline spline)
	{
		Vector2[] corners = new Vector2[spline.GetPointCount()];
		for (int i = 0; i < corners.Length; i ++)
			corners[i] = spline.GetPosition(i);
		return FromCornersWithLoop(corners);
	}
#endif

	public static Shape2D FromEdgesWithLoop (params LineSegment2D[] edges)
	{
		Shape2D output = new Shape2D();
		output.edges = new LineSegment2D[edges.Length];
		edges.CopyTo(output.edges, 0);
		output.SetCorners_Polygon ();
		return output;
	}

	public static Shape2D FromCornersWithLoop (params Vector2[] corners)
	{
		Shape2D output = new Shape2D();
		output.corners = new Vector2[corners.Length];
		corners.CopyTo(output.corners, 0);
		output.SetEdges_Polygon ();
		return output;
	}

	public static Shape2D Polygon (float edgeCount, float rotation = 0, float radius = .5f)
	{
		throw new NotImplementedException();
	}

	public static Shape2D RegularPolygon (int edgeCount, float rotation = 0, float radius = .5f)
	{
		Vector2[] outputCorners = new Vector2[edgeCount];
		for (int i = 0; i < edgeCount; i ++)
			outputCorners[i] = VectorExtensions.FromFacingAngle(360f / edgeCount * i) * radius;
		return FromCornersWithLoop(outputCorners);
	}

	public static bool operator== (Shape2D shape, Shape2D shape2)
	{
		if (shape.corners.Length != shape2.corners.Length || shape.edges.Length != shape2.edges.Length)
			return false;
		Vector2 firstCorner = shape.corners[0];
		int indexOffset = 0;
		for (int i = 0; i < shape2.corners.Length; i ++)
		{
			Vector2 corner = shape2.corners[i];
			if (corner == firstCorner)
			{
				indexOffset = i;
				break;
			}
		}
		for (int i = 0; i < shape.corners.Length; i ++)
		{
			Vector2 corner = shape.corners[i];
			if (corner != shape2.corners[(i + indexOffset) % shape.corners.Length])
				return false;
		}
		return true;
	}

	public static bool operator!= (Shape2D shape, Shape2D shape2)
	{
		if (shape.corners.Length != shape2.corners.Length || shape.edges.Length != shape2.edges.Length)
			return true;
		Vector2 firstCorner = shape.corners[0];
		int indexOffset = 0;
		for (int i = 0; i < shape2.corners.Length; i ++)
		{
			Vector2 corner = shape2.corners[i];
			if (corner == firstCorner)
			{
				indexOffset = i;
				break;
			}
		}
		for (int i = 0; i < shape.corners.Length; i ++)
		{
			Vector2 corner = shape.corners[i];
			if (corner != shape2.corners[(i + indexOffset) % shape.corners.Length])
				return true;
		}
		return false;
	}
}