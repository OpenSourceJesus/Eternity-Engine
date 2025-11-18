using UnityEngine;

namespace EternityEngine
{
	public class Value<T> : MonoBehaviour
	{
		public T val;
		public delegate void OnChanged();
		public event OnChanged onChanged;
		public _Component component;
		[HideInInspector]
		public SetableValue[] setableValues = new SetableValue[0];

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
				for (int i = 0; i < setableValues.Length; i ++)
				{
					SetableValue setableValue = setableValues[i];
					setableValue.valueSetter.text =  "" + val;
				}
		}

		public void _OnChanged ()
		{
			onChanged ();
		}
	}
}