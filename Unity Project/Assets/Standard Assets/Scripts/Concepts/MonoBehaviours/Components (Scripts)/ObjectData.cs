using System;

namespace EternityEngine
{
	public class ObjectData : _Component
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
		public BoolValue export;

		[Serializable]
		public class Data : _Component.Data
		{
			public bool export;

			public override void Set (_Component component)
			{
				base.Set (component);
				ObjectData obData = (ObjectData) component;
				export = obData.export.val;
			}

			public override void Apply (_Component component)
			{
				base.Apply (component);
				ObjectData obData = (ObjectData) component;
				obData.export.Set (export);
			}
		}
	}
}