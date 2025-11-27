using System;
using Extensions;
using UnityEngine;

namespace EternityEngine
{
	public class _Collider : _Component
	{
		public new Data _Data
		{
			get
			{
				if (data == null)
					data = new Data();
				return (Data) data;
			}
			set
			{
				data = value;
			}
		}
		public IntValue type;
		public BoolArrayValue collisionGroupMembership;
		public BoolArrayValue collisionGroupFilter;
		public FloatValue radius;
		public Vector2Value normal;
		public Vector2Value size;
		public FloatValue cuboidBorderRadius;
		public FloatValue triangleBorderRadius;
		public FloatValue capsuleHeight;
		public FloatValue capsuleRadius;
		public BoolValue isVertical;
		public Vector2Value segmentPnt0;
		public Vector2Value segmentPnt1;
		public Vector2Value trianglePnt0;
		public Vector2Value trianglePnt1;
		public Vector2Value trianglePnt2;
		public Vector2ArrayValue polylinePnts;
		public IntArrayValue polylineIdxs;
		public Vector2ArrayValue trimeshPnts;
		public IntArrayValue trimeshIdxs;
		public Vector2ArrayValue convexHullPnts;
		public FloatValue convexHullBorderRadius;
		public Vector2Value heightfieldScale;
		public FloatArrayValue heights;
		public BoolValue isSensor;
		public FloatValue density;
		public FloatValue bounciness;
		public StringValue bouncinessCombineRule;

		[Serializable]
		public class Data : _Component.Data
		{
			public int type;
			public float radius;
			public bool[] collisionGroupMembership = new bool[0];
			public bool[] collisionGroupFilter = new bool[0];
			public bool isSensor;
			public float density;
			public float bounciness;
			public string bouncinessCombineRule;
			public _Vector2 normal;
			public _Vector2 size;
			public float cuboidBorderRadius;
			public float triangleBorderRadius;
			public float capsuleHeight;
			public float capsuleRadius;
			public bool isVertical;
			public _Vector2 segmentPnt0;
			public _Vector2 segmentPnt1;
			public _Vector2 trianglePnt0;
			public _Vector2 trianglePnt1;
			public _Vector2 trianglePnt2;
			public _Vector2[] polylinePnts = new _Vector2[0];
			public int[] polylineIdxs;
			public _Vector2[] trimeshPnts = new _Vector2[0];
			public int[] trimeshIdxs;
			public _Vector2[] convexHullPnts = new _Vector2[0];
			public float convexHullBorderRadius;
			public _Vector2 heightfieldScale;
			public float[] heights = new float[0];

			public override void Set (_Component component)
			{
				base.Set (component);
				_Collider collider = (_Collider) component;
				type = collider.type.val;
				radius = collider.radius.val;
				normal = _Vector2.FromVec2(collider.normal.val);
				isSensor = collider.isSensor.val;
				density = collider.density.val;
				bounciness = collider.bounciness.val;
				bouncinessCombineRule = collider.bouncinessCombineRule.val;
				convexHullBorderRadius = collider.convexHullBorderRadius.val;
				heightfieldScale = _Vector2.FromVec2(collider.heightfieldScale.val);
				heights = collider.heights.val;
				collisionGroupMembership = collider.collisionGroupMembership.val;
				collisionGroupFilter = collider.collisionGroupFilter.val;
			}

			public override void Apply (_Component component)
			{
				base.Apply (component);
				_Collider collider = (_Collider) component;
				collider.type.Set (type);
				collider.radius.Set (radius);
				collider.segmentPnt0.Set (segmentPnt0.ToVec2());
				collider.segmentPnt1.Set (segmentPnt1.ToVec2());
				collider.trianglePnt0.Set (trianglePnt0.ToVec2());
				collider.trianglePnt1.Set (trianglePnt1.ToVec2());
				collider.trianglePnt2.Set (trianglePnt2.ToVec2());
				collider.isSensor.Set (isSensor);
				collider.normal.Set (normal.ToVec2());
				collider.collisionGroupMembership.Set (collisionGroupMembership);
				collider.collisionGroupFilter.Set (collisionGroupFilter);
				collider.size.Set (size.ToVec2());
				collider.cuboidBorderRadius.Set (cuboidBorderRadius);
				collider.triangleBorderRadius.Set (triangleBorderRadius);
				collider.capsuleHeight.Set (capsuleHeight);
				collider.capsuleRadius.Set (capsuleRadius);
				collider.isVertical.Set (isVertical);
				collider.polylinePnts.Set (polylinePnts.ToVec2s());
				collider.polylineIdxs.Set (polylineIdxs);
				collider.trimeshPnts.Set (trimeshPnts.ToVec2s());
				collider.trimeshIdxs.Set (trimeshIdxs);
				collider.convexHullPnts.Set (convexHullPnts.ToVec2s());
				collider.convexHullBorderRadius.Set (convexHullBorderRadius);
				collider.heightfieldScale.Set (heightfieldScale.ToVec2());
				collider.heights.Set (heights);
			}
		}
	}
}