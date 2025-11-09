using TMPro;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace EternityEngine
{
	public class HierarchyEntry : MonoBehaviour
	{
		public RectTransform rectTrs;
		[HideInInspector]
		public _Object ob;
		public TMP_Text nameText;
		public Image selectedIndicator;
		[HideInInspector]
		public HierarchyPanel hierarchyPanel;
		[HideInInspector]
		public bool selected;
		int insertAt;

		public void OnMouseDown ()
		{
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				HierarchyEntry whatToSelect = hierarchyPanel.entries[rectTrs.GetSiblingIndex()];
				for (int i2 = 0; i2 < hierarchyPanel.selected.Length; i2 ++)
				{
					HierarchyEntry hierarchyEntry = hierarchyPanel.selected[i2];
					if (hierarchyEntry != whatToSelect)
						hierarchyEntry.SetSelected (false);
				}
				if (!whatToSelect.selected)
					whatToSelect.SetSelected (true);
			}
		}

		public void BeginDrag ()
		{
			hierarchyPanel.insertionIndicator = Instantiate(EternityEngine.instance.insertionIndicatorPrefab, hierarchyPanel.entriesParent);
			hierarchyPanel.insertionIndicator.rectTransform.sizeDelta = hierarchyPanel.insertionIndicator.rectTransform.sizeDelta.SetX(0);
		}

		public void Drag ()
		{
			Vector2 mousePos = Mouse.current.position.ReadValue();
			Rect parentWorldRect = hierarchyPanel.entriesParent.GetWorldRect();
			hierarchyPanel.insertionIndicator.enabled = parentWorldRect.Contains(mousePos);
			if (hierarchyPanel.insertionIndicator.enabled)
				for (int i = 0; i < hierarchyPanel.entries.Length; i ++)
				{
					HierarchyEntry hierarchyEntry = hierarchyPanel.entries[i];
					Rect hierarchyEntryWorldRect = hierarchyEntry.rectTrs.GetWorldRect();
					if (hierarchyEntryWorldRect.Contains(mousePos))
					{
						if (Rect.PointToNormalized(hierarchyEntryWorldRect, mousePos).y > .5f)
						{
							hierarchyPanel.insertionIndicator.rectTransform.position = new Vector3(hierarchyEntryWorldRect.center.x, hierarchyEntryWorldRect.yMax);
							if (i > rectTrs.GetSiblingIndex())
								insertAt = i - 1;
							else
								insertAt = i;
						}
						else
						{
							hierarchyPanel.insertionIndicator.rectTransform.position = new Vector3(hierarchyEntryWorldRect.center.x, hierarchyEntryWorldRect.yMin);
							if (i > rectTrs.GetSiblingIndex())
								insertAt = i;
							else
								insertAt = i + 1;
						}
					}
				}
		}

		public void EndDrag ()
		{
			Destroy(hierarchyPanel.insertionIndicator.gameObject);
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				HierarchyEntry hierarchyEntry = hierarchyPanel.entries[rectTrs.GetSiblingIndex()];
				hierarchyPanel.entries = hierarchyPanel.entries.RemoveAt(rectTrs.GetSiblingIndex());
				hierarchyPanel.entries = hierarchyPanel.entries.Insert(hierarchyEntry, insertAt);
				hierarchyPanel.entriesParent.GetChild(rectTrs.GetSiblingIndex()).SetSiblingIndex(insertAt);
			}
		}

		void SetSelected (bool select)
		{
			if (select && !selected)
				hierarchyPanel.selected = hierarchyPanel.selected.Add(this);
			else if (!select && selected)
				hierarchyPanel.selected = hierarchyPanel.selected.Remove(this);
			selectedIndicator.enabled = select;
			selected = select;
		}
	}
}