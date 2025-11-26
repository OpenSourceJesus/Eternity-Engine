using System;
using Extensions;

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
		public BoolValue[] collisionGroupMembership = new BoolValue[0];
		public BoolValue[] collisionGroupFilter = new BoolValue[0];
		public FloatValue radius;

		[Serializable]
		public class Data : _Component.Data
		{
			public int type;
			public float radius;
			public bool[] collisionGroupMembership = new bool[0];
			public bool[] collisionGroupFilter = new bool[0];

			public override void Set (_Component component)
			{
				base.Set (component);
				_Collider collider = (_Collider) component;
				type = collider.type.val;
				radius = collider.radius.val;
				collisionGroupMembership = new bool[collider.collisionGroupMembership.Length];
				for (int i = 0; i < collider.collisionGroupMembership.Length; i ++)
				{
					BoolValue isCollisionGroupMember = collider.collisionGroupMembership[i];
					collisionGroupMembership[i] = isCollisionGroupMember.val;
				}
				collisionGroupFilter = new bool[collider.collisionGroupFilter.Length];
				for (int i = 0; i < collider.collisionGroupFilter.Length; i ++)
				{
					BoolValue collideWithCollisionGroup = collider.collisionGroupFilter[i];
					collisionGroupFilter[i] = collideWithCollisionGroup.val;
				}
			}

			public override void Apply (_Component component)
			{
				base.Apply (component);
				_Collider collider = (_Collider) component;
				collider.type.Set (type);
				collider.radius.Set (radius);
				for (int i = 0; i < collisionGroupMembership.Length; i ++)
				{
					bool isCollisionGroupMember = collisionGroupMembership[i];
					BoolValue isCollisionGroupMemberValue = collider.collisionGroupMembership[i];
					isCollisionGroupMemberValue.Set (isCollisionGroupMember);
				}
				for (int i = 0; i < collisionGroupFilter.Length; i ++)
				{
					bool isCollisionGroupMember = collisionGroupFilter[i];
					BoolValue collideWithCollisionGroup = collider.collisionGroupFilter[i];
					collideWithCollisionGroup.Set (isCollisionGroupMember);
				}
			}
		}
	}
}