using System;
using Extensions;

namespace EternityEngine
{
	public class Script : _Component
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
		public StringValue path;
		public BoolValue runtime;
		public BoolValue runAtStart;

		[Serializable]
		public class Data : _Component.Data
		{
			public string path;
			public bool runtime;
			public bool runAtStart;

			public override void Set (_Component component)
			{
				base.Set (component);
				Script script = (Script) component;
				path = script.path.val;
				runtime = script.runtime.val;
				runAtStart = script.runAtStart.val;
			}

			public override void Apply (_Component component)
			{
				base.Apply (component);
				Script script = (Script) component;
				script.path.Set (path);
				script.runtime.Set (runtime);
				script.runAtStart.Set (runAtStart);
			}
		}
	}
}