using UnityEngine;

namespace EternityEngine
{
	public class Value<T> : MonoBehaviour
	{
		public T val;
		public delegate void OnChanged();
		public event OnChanged onChanged = () => {};
		public _Component component;
		public ValueEntry<T>[] entries = new ValueEntry<T>[0];

		public virtual void Awake ()
		{
			if (component != null)
				onChanged += HandleChange;
		}

		public virtual void OnDestroy ()
		{
			if (component != null)
				onChanged -= HandleChange;
		}

		void HandleChange ()
		{
			if (!component.inspectorEntries[0].gameObject.activeSelf)
				for (int i = 0; i < entries.Length; i ++)
				{
					ValueEntry<T> entry = entries[i];
					entry.UpdateDisplay (val);
				}
		}

		public void _OnChanged ()
		{
			onChanged ();
		}

		public virtual void Set (T val)
		{
			T prevVal = this.val;
			this.val = val;
			for (int i = 0; i < entries.Length; i ++)
			{
				ValueEntry<T> entry = entries[i];
				entry.UpdateDisplay (val);
			}
			if (!prevVal.Equals(val))
				onChanged ();
		}
	}
}