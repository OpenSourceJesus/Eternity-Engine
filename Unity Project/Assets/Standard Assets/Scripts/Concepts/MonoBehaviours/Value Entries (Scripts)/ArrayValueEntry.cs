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
		public GameObject addAndRemoveButtonsParentGo;
		public GameObject resizeInputFieldGo;
		ValueEntry<T> dragging;
		Vector2 offDrag;

		void Start ()
		{
			if (((ArrayValue<T>) value).canResize)
			{
				addAndRemoveButtonsParentGo.SetActive(true);
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
			int size = int.Parse(sizeStr);
			if (size < elts.Length)
				for (int i = elts.Length - 1; i <= size; i --)
					RemoveElement (i);
			else if (size > elts.Length)
				for (int i = elts.Length; i < size; i ++)
					AddElement ();
		}

		public void AddElement ()
		{
			ValueEntry<T> elt = Instantiate(eltPrefab, eltsParent);
			elts = elts.Add(elt);
			elt.onMouseDown += OnElementMouseDown;
			elt.onMouseUp += OnElementMouseUp;
		}

		void RemoveElement (int idx)
		{
			ValueEntry<T> elt = elts[idx];
			elts = elts.RemoveAt(idx);
			elt.onMouseDown -= OnElementMouseDown;
			elt.onMouseUp -= OnElementMouseUp;
			DestroyImmediate(elt.gameObject);
		}

		public void RemoveSelectedElements ()
		{
			for (int i = 0; i < elts.Length; i ++)
			{
				ValueEntry<T> elt = elts[i];
				if (elt.selected)
					RemoveElement (elt.rectTrs.GetSiblingIndex());
			}
		}

		public void DuplicateElement (int idx)
		{
			ValueEntry<T> elt = Instantiate(elts[idx], eltsParent);
			InsertElement (elt, idx + 1);
			elt.onMouseDown += OnElementMouseDown;
		}

		void InsertElement (ValueEntry<T> elt, int idx)
		{
			elt.rectTrs.SetSiblingIndex(idx);
			elts = elts.Insert(elt, idx);
		}

		void OnElementMouseDown (ValueEntry<T> elt)
		{
			dragging = elt;
			offDrag = (Vector2) elt.rectTrs.position - Mouse.current.position.ReadValue();
			elt.layoutElt.ignoreLayout = true;
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		void OnElementMouseUp (ValueEntry<T> elt)
		{
			elt.layoutElt.ignoreLayout = false;
			GameManager.updatables = GameManager.updatables.Remove(this);
			dragging = null;
		}
	}
}