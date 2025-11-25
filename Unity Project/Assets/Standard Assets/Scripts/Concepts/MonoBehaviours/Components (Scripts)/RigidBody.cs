using System;
using Extensions;

namespace EternityEngine
{
	public class RigidBody : _Component
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
		public FloatValue gravityScale;
		public IntValue dominance;
		public BoolValue canRot;
		public FloatValue linearDrag;
		public FloatValue angDrag;
		public BoolValue canSleep;
		public BoolValue continuousCollideDetect;

		[Serializable]
		public class Data : _Component.Data
		{
			public int type;
			public float gravityScale;
			public int dominance;
			public bool canRot;
			public float linearDrag;
			public float angDrag;
			public bool canSleep;
			public bool continuousCollideDetect;

			public override void Set (_Component component)
			{
				base.Set (component);
				RigidBody rigidBody = (RigidBody) component;
				type = rigidBody.type.val;
				gravityScale = rigidBody.gravityScale.val;
				dominance = rigidBody.dominance.val;
				canRot = rigidBody.canRot.val;
				linearDrag = rigidBody.linearDrag.val;
				angDrag = rigidBody.angDrag.val;
				canSleep = rigidBody.canSleep.val;
				continuousCollideDetect = rigidBody.continuousCollideDetect.val;
			}

			public override void Apply (_Component component)
			{
				base.Apply (component);
				RigidBody rigidBody = (RigidBody) component;
				rigidBody.type.Set (type);
				rigidBody.gravityScale.Set (gravityScale);
				rigidBody.dominance.Set (dominance);
				rigidBody.canRot.Set (canRot);
				rigidBody.linearDrag.Set (linearDrag);
				rigidBody.angDrag.Set (angDrag);
				rigidBody.canSleep.Set (canSleep);
				rigidBody.continuousCollideDetect.Set (continuousCollideDetect);
			}
		}
	}
}