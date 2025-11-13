using UnityEngine;

namespace EternityEngine
{
	public class _Image : _Component
	{
		public StringValue path;

		void Awake ()
		{
			path.onChanged += () => {  };
		}

		void OnDestroy ()
		{
			path.onChanged -= () => {  };
		}
	}
}