using TMPro;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace EternityEngine
{
	public class AttributeValueEntry<T> : ValueEntry<Dictionary<string, T>>, IUpdatable
	{
		public ValueEntry<KeyValuePair<string, T>> eltPrefab;
		public Transform eltsParent;
		public ValueEntry<KeyValuePair<string, T>>[] elts = new ValueEntry<KeyValuePair<string, T>>[0];
		public RectTransform collapseButtonRectTrs;
		public TMP_InputField resizeInputField;
		ValueEntry<KeyValuePair<string, T>> dragging;
		Vector2 offDrag;

		public void DoUpdate ()
		{
			Vector2 mousePos = Mouse.current.position.ReadValue();
			dragging.rectTrs.position = offDrag + mousePos;
			if (Mouse.current.leftButton.wasReleasedThisFrame)
			{
				for (int i = 0; i < elts.Length; i ++)
				{
					ValueEntry<KeyValuePair<string, T>> elt = elts[i];
					if (elt.rectTrs.GetWorldRect().Contains(mousePos))
					{
						elts = elts.RemoveAt(dragging.rectTrs.GetSiblingIndex());
						InsertElement (dragging, i);
						break;
					}
				}
			}
		}

		public override void UpdateDisplay (Dictionary<string, T> val)
		{
			Resize (val.Count, true);
			int i = 0;
			foreach (KeyValuePair<string, T> keyValuePair in val)
			{
				ValueEntry<KeyValuePair<string, T>> elt = elts[i];
				elt.UpdateDisplay (keyValuePair);
				i ++;
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
			ValueEntry<KeyValuePair<string, T>> elt = Instantiate(eltPrefab, eltsParent);
			elts = elts.Add(elt);
			elt.onMouseDown += OnElementMouseDown;
			elt.onMouseUp += OnElementMouseUp;
			elt.removeButton.onClick.AddListener(() => { RemoveElement (elt.rectTrs.GetSiblingIndex()); });
			elt.removeButtonGo.SetActive(true);
			elt.draggableIndicator.SetActive(true);
			resizeInputField.text = "" + elts.Length;
			if (justAFfectDisplay)
				return;
			Value<Dictionary<string, T>>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			Dictionary<string, T> val = new Dictionary<string, T>(value.val);
			val.Add("", default(T));
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<Dictionary<string, T>> target = targets[i];
				if (target == null)
					continue;
				target.val = val;
				target._OnChanged ();
			}
		}

		void RemoveElement (int idx, bool justAFfectDisplay = false)
		{
			ValueEntry<KeyValuePair<string, T>> elt = elts[idx];
			elts = elts.RemoveAt(idx);
			elt.onMouseDown -= OnElementMouseDown;
			elt.onMouseUp -= OnElementMouseUp;
			DestroyImmediate(elt.gameObject);
			resizeInputField.text = "" + elts.Length;
			if (justAFfectDisplay)
				return;
			Value<Dictionary<string, T>>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			Dictionary<string, T> val = new Dictionary<string, T>(value.val);
			val.Remove(val.GetKey(idx));
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<Dictionary<string, T>> target = targets[i];
				if (target == null)
					continue;
				target.val = val;
				target._OnChanged ();
			}
		}

		public void DuplicateElement (int idx)
		{
			ValueEntry<KeyValuePair<string, T>> elt = Instantiate(elts[idx], eltsParent);
			InsertElement (elt, idx + 1);
			elt.onMouseDown += OnElementMouseDown;
			resizeInputField.text = "" + elts.Length;
		}

		void InsertElement (ValueEntry<KeyValuePair<string, T>> elt, int idx)
		{
			elt.rectTrs.SetSiblingIndex(idx);
			elts = elts.Insert(elt, idx);
			
		}

		public void OnElementMouseDown (ValueEntry<KeyValuePair<string, T>> elt)
		{
			dragging = elt;
			offDrag = (Vector2) elt.rectTrs.position - Mouse.current.position.ReadValue();
			elt.selectedIndicator.SetActive(true);
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void OnElementMouseUp (ValueEntry<KeyValuePair<string, T>> elt)
		{
			elt.selectedIndicator.SetActive(false);
			GameManager.updatables = GameManager.updatables.Remove(this);
			dragging = null;
		}

		public void OnElementValueChanged (ValueEntry<KeyValuePair<string, T>> elt)
		{
			int idx = elt.rectTrs.GetSiblingIndex();
			Value<Dictionary<string, T>>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			Dictionary<string, T> val = null;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<Dictionary<string, T>> target = targets[i];
				if (target == null)
					continue;
				Dictionary<string, T> prevVal = target.val;
				target.val.Remove(target.val.GetKey(idx));
				KeyValuePair<string, T> keyValuePair = elt.GetValue();
				target.val.Add(keyValuePair.Key, keyValuePair.Value);
				val = target.val;
				if (!prevVal.Equals(target.val))
					target._OnChanged ();
			}
			UpdateDisplay (val);
		}

		public new void TrySet (Dictionary<string, T> val)
		{
			Value<Dictionary<string, T>>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<Dictionary<string, T>> target = targets[i];
				if (target == null)
					continue;
				Dictionary<string, T> prevVal = target.val;
				target.val = val;
				if (!prevVal.Equals(val))
					target._OnChanged ();
			}
			UpdateDisplay (val);
		}
	}
}