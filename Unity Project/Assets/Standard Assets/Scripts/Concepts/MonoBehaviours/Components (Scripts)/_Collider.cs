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
		public ArrayValue<bool> collisionGroupMembership;
		public ArrayValue<bool> collisionGroupFilter;
		public FloatValue radius;
		public Vector2Value normal;
		public Vector2Value size;
		public FloatValue cuboidBorderRadius;
		public FloatValue triangleBorderRadius;
		public FloatValue capsuleHeight;
		public FloatValue capsuleRadius;
		public BoolValue isVertical;
		public ArrayValue<Vector2> polylinePnts;
		public ArrayValue<int> polylineIdxs;
		public ArrayValue<Vector2> trimeshPnts;
		public ArrayValue<int> trimeshIdxs;
		public ArrayValue<Vector2> convexHullPnts;
		public FloatValue convexHullBorderRadius;
		public Vector2Value heightfieldScale;
		public ArrayValue<float> heightfieldHeights;
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
			public _Vector2[] polylinePnts = new _Vector2[0];
			public int[] polylineIdxs;
			public _Vector2[] trimeshPnts = new _Vector2[0];
			public int[] trimeshIdxs;
			public _Vector2[] convexHullPnts = new _Vector2[0];
			public float convexHullBorderRadius;
			public _Vector2 heightfieldScale;
			public float[] heightfieldHeights = new float[0];

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
				heightfieldHeights = collider.heightfieldHeights.val;
				collisionGroupMembership = collider.collisionGroupMembership.val;
				collisionGroupFilter = collider.collisionGroupFilter.val;
			}

			public override void Apply (_Component component)
			{
				base.Apply (component);
				_Collider collider = (_Collider) component;
				collider.type.Set (type);
				collider.radius.Set (radius);
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
				collider.heightfieldHeights.Set (heightfieldHeights);
			}
		}
	}
}