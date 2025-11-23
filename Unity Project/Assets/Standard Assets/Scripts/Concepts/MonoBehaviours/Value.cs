using UnityEngine;

namespace EternityEngine
{
	public class Value<T> : MonoBehaviour
	{
		public T val;
		public delegate void OnChanged();
		public event OnChanged onChanged = () => {};
		public _Component component;
		[HideInInspector]
		public ValueSetter[] setters = new ValueSetter[0];

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
				for (int i = 0; i < setters.Length; i ++)
				{
					ValueSetter setableValue = setters[i];
					setableValue.setter.text = "" + val;
				}
		}

		public void _OnChanged ()
		{
			onChanged ();
		}
	}
}