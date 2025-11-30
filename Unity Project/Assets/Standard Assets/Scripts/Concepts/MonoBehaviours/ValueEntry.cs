using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class ValueEntry<T> : ValueSetter
	{
		public RectTransform rectTrs;
		public GameObject draggableIndicator;
		public GameObject selectedIndicator;
		public Button removeButton;
		public GameObject removeButtonGo;
		public Value<T> value;
		public Value<T>[] values = new Value<T>[0];
		public delegate void OnMouseDown(ValueEntry<T> valueEntry);
		public event OnMouseDown onMouseDown;
		public delegate void OnMouseUp(ValueEntry<T> valueEntry);
		public event OnMouseDown onMouseUp;
		protected Value<T>[] TargetValues
		{
			get
			{
				if (values != null && values.Length > 0)
					return values;
				if (value != null)
					return new Value<T>[] { value };
				return new Value<T>[0];
			}
		}

		public void SetValues (params Value<T>[] values)
		{
			if (values != null && values.Length > 0)
				this.values = values;
			else
				this.values = new Value<T>[0];
			if (this.values.Length > 0)
				value = this.values[0];
			else
				value = null;
		}

		public void DetachValues ()
		{
			Value<T>[] targets = TargetValues;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<T> target = targets[i];
				if (target != null)
					target.entries = target.entries.Remove(this);
			}
			values = new Value<T>[0];
			value = null;
		}

		public virtual void UpdateDisplay (T val)
		{
			if (inputField != null)
				inputField.text = "" + val;
		}

		public virtual T GetValue ()
		{
			throw new NotImplementedException();
		}

		public void _OnMouseDown ()
		{
			if (onMouseDown != null)
				onMouseDown (this);
		}

		public void _OnMouseUp ()
		{
			if (onMouseUp != null)
				onMouseUp (this);
		}
	}
}