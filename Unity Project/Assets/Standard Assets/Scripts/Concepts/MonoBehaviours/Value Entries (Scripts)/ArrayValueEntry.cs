using TMPro;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace EternityEngine
{
	public class ArrayValueEntry<T> : ValueEntry<T[]>, IUpdatable
	{
		public ValueEntry<T> eltPrefab;
		public Transform eltsParent;
		public ValueEntry<T>[] elts = new ValueEntry<T>[0];
		public RectTransform collapseButtonRectTrs;
		public GameObject addEltButtonGo;
		public TMP_InputField resizeInputField;
		public GameObject resizeInputFieldGo;
		ValueEntry<T> dragging;
		Vector2 offDrag;

		public virtual void Start ()
		{
			if (((ArrayValue<T>) value).canResize)
			{
				addEltButtonGo.SetActive(true);
				resizeInputFieldGo.SetActive(true);
			}
		}

		public void DoUpdate ()
		{
			Vector2 mousePos = Mouse.current.position.ReadValue();
			dragging.rectTrs.position = offDrag + mousePos;
			if (Mouse.current.leftButton.wasReleasedThisFrame)
			{
				for (int i = 0; i < elts.Length; i ++)
				{
					ValueEntry<T> elt = elts[i];
					if (elt.rectTrs.GetWorldRect().Contains(mousePos))
					{
						elts = elts.RemoveAt(dragging.rectTrs.GetSiblingIndex());
						InsertElement (dragging, i);
						break;
					}
				}
			}
		}

		public override void UpdateDisplay (T[] val)
		{
			Resize (val.Length, true);
			for (int i = 0; i < elts.Length; i ++)
			{
				ValueEntry<T> elt = elts[i];
				elt.UpdateDisplay(val[i]);
			}
		}

		void SetCollapsed (bool collapse)
		{
			eltsParent.gameObject.SetActive(!collapse);
			collapseButtonRectTrs.eulerAngles = Vector3.forward * 180 * collapse.GetHashCode();
		}

		public void ToggleCollapse ()
		{
			SetCollapsed (eltsParent.gameObject.activeSelf);
		}

		public void Resize (string sizeStr)
		{
			if (sizeStr == "")
				sizeStr = "0";
			Resize (int.Parse(sizeStr));
		}

		void Resize (int size, bool justAFfectDisplay = false)
		{
			if (size < elts.Length)
				for (int i = elts.Length - 1; i >= size; i --)
					RemoveElement (i, justAFfectDisplay);
			else if (size > elts.Length)
				for (int i = elts.Length; i < size; i ++)
					AddElement (justAFfectDisplay);
		}

		public void AddElement (bool justAFfectDisplay = false)
		{
			ValueEntry<T> elt = Instantiate(eltPrefab, eltsParent);
			elts = elts.Add(elt);
			elt.onMouseDown += OnElementMouseDown;
			elt.onMouseUp += OnElementMouseUp;
			elt.removeButton.onClick.AddListener(() => { RemoveElement (elt.rectTrs.GetSiblingIndex()); });
			elt.removeButtonGo.SetActive(true);
			elt.draggableIndicator.SetActive(true);
			resizeInputField.text = "" + elts.Length;
			if (justAFfectDisplay)
				return;
			Value<T[]>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			T[] val = value.val.Add(default(T));
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<T[]> target = targets[i];
				if (target == null)
					continue;
				target.val = val;
				target._OnChanged ();
			}
		}

		void RemoveElement (int idx, bool justAFfectDisplay = false)
		{
			ValueEntry<T> elt = elts[idx];
			elts = elts.RemoveAt(idx);
			elt.onMouseDown -= OnElementMouseDown;
			elt.onMouseUp -= OnElementMouseUp;
			DestroyImmediate(elt.gameObject);
			resizeInputField.text = "" + elts.Length;
			if (justAFfectDisplay)
				return;
			Value<T[]>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			T[] val = value.val.RemoveAt(idx);
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<T[]> target = targets[i];
				if (target == null)
					continue;
				target.val = val;
				target._OnChanged ();
			}
		}

		public void DuplicateElement (int idx)
		{
			ValueEntry<T> elt = Instantiate(elts[idx], eltsParent);
			InsertElement (elt, idx + 1);
			elt.onMouseDown += OnElementMouseDown;
			resizeInputField.text = "" + elts.Length;
		}

		void InsertElement (ValueEntry<T> elt, int idx)
		{
			elt.rectTrs.SetSiblingIndex(idx);
			elts = elts.Insert(elt, idx);
			
		}

		public void OnElementMouseDown (ValueEntry<T> elt)
		{
			dragging = elt;
			offDrag = (Vector2) elt.rectTrs.position - Mouse.current.position.ReadValue();
			elt.selectedIndicator.SetActive(true);
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void OnElementMouseUp (ValueEntry<T> elt)
		{
			elt.selectedIndicator.SetActive(false);
			GameManager.updatables = GameManager.updatables.Remove(this);
			dragging = null;
		}

		public void OnElementValueChanged (ValueEntry<T> elt)
		{
			int idx = elt.rectTrs.GetSiblingIndex();
			Value<T[]>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			T[] val = null;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<T[]> target = targets[i];
				if (target == null)
					continue;
				T[] prevVal = target.val;
				target.val[idx] = elt.value.val;
				val = target.val;
				if (!prevVal.Equals(target.val))
					target._OnChanged ();
			}
			UpdateDisplay (val);
		}

		public new void TrySet (T[] val)
		{
			Value<T[]>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<T[]> target = targets[i];
				if (target == null)
					continue;
				T[] prevVal = target.val;
				target.val = val;
				if (!prevVal.Equals(val))
					target._OnChanged ();
			}
			UpdateDisplay (val);
		}
	}
}