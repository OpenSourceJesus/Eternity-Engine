using UnityEngine;

namespace EternityEngine
{
	public class Value<T> : MonoBehaviour
	{
		public T val;
		public delegate void OnChanged();
		public event OnChanged onChanged;
		public _Component component;
		public SetableValue setableValue;

		public virtual void Awake ()
		{
			onChanged += () => { component.setValueEntriesTo[setableValue] = "" + val; };
		}

		public virtual void OnDestroy ()
		{
			onChanged -= () => { component.setValueEntriesTo[setableValue] = "" + val; };
		}

		public void _OnChanged ()
		{
			onChanged ();
		}
	}
}