using System;
using System.Collections.Generic;

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
		public BoolAttributeValue boolAttributes;
		public StringAttributeValue stringAttributes;

		[Serializable]
		public class Data : _Component.Data
		{
			public bool export;
			public Dictionary<string, bool> boolAttributes = new Dictionary<string, bool>();
			public Dictionary<string, string> stringAttributes = new Dictionary<string, string>();

			public override void Set (_Component component)
			{
				base.Set (component);
				ObjectData obData = (ObjectData) component;
				export = obData.export.val;
				boolAttributes = obData.boolAttributes.val;
				stringAttributes = obData.stringAttributes.val;
			}

			public override void Apply (_Component component)
			{
				base.Apply (component);
				ObjectData obData = (ObjectData) component;
				obData.export.Set (export);
				obData.boolAttributes.Set (boolAttributes);
				obData.stringAttributes.Set (stringAttributes);
			}
		}
	}
}