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
		public LayoutElement layoutElement;
		public ValueEntry<T>[] elts = new ValueEntry<T>[0];
		ValueEntry<T> dragging;
		Vector2 offDrag;

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

		public void AddElement ()
		{
			ValueEntry<T> elt = Instantiate(eltPrefab, eltsParent);
			elts = elts.Add(elt);
			elt.onMouseDown += OnElementMouseDown;
			elt.onMouseUp += OnElementMouseUp;
		}

		public void RemoveElement (int idx)
		{
			ValueEntry<T> elt = elts[idx];
			Destroy(elt.gameObject);
			elts = elts.RemoveAt(idx);
			elt.onMouseDown -= OnElementMouseDown;
			elt.onMouseUp -= OnElementMouseUp;
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
			layoutElement.ignoreLayout = true;
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		void OnElementMouseUp (ValueEntry<T> elt)
		{
			layoutElement.ignoreLayout = false;
			GameManager.updatables = GameManager.updatables.Remove(this);
			dragging = null;
		}
	}
}