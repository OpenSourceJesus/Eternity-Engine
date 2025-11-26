using TMPro;
using Extensions;
using UnityEngine;

namespace EternityEngine
{
	public class ArrayValueEntry<T> : ValueEntry<T[]>
	{
		public ValueEntry<T> eltPrefab;
		public Transform eltsParent;
		[HideInInspector]
		ValueEntry<T>[] elts = new ValueEntry<T>[0];

		public void AddElement ()
		{
			ValueEntry<T> elt = Instantiate(eltPrefab, eltsParent);
			elts = elts.Add(elt);
		}

		public void RemoveElement (int idx)
		{
			Destroy(elts[idx].gameObject);
			elts = elts.RemoveAt(idx);
		}

		public void DuplicateElement (int idx)
		{
			ValueEntry<T> elt = Instantiate(elts[idx], eltsParent);
			elt.trs.SetSiblingIndex(idx + 1);
			elts = elts.Insert(elt, idx + 1);
		}
	}
}