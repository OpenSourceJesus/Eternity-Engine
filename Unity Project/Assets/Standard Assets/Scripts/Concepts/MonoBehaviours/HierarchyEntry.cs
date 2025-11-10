using TMPro;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
				int idx = rectTrs.GetSiblingIndex();
				HierarchyEntry clickedHierarchyEntry = hierarchyPanel.entries[idx];
				if (Keyboard.current.leftShiftKey.isPressed)
				{
					int lastEntryIdxHadSelectionSet = HierarchyPanel.lastEntryIdxHadSelectionSet;
					if (lastEntryIdxHadSelectionSet == -1)
						clickedHierarchyEntry.SetSelected (true);
					else
					{
						if (idx > lastEntryIdxHadSelectionSet)
						{
							int startIdx = lastEntryIdxHadSelectionSet + 1;
							int endIdx = idx;
							if (clickedHierarchyEntry.selected)
							{
								startIdx --;
								endIdx --;
							}
							else if (!hierarchyPanel.entries[startIdx].selected)
								startIdx --;
							for (int i2 = startIdx; i2 <= endIdx; i2 ++)
							{
								HierarchyEntry hierarchyEntry = hierarchyPanel.entries[i2];
								if (!hierarchyEntry.selected && !clickedHierarchyEntry.selected)
									HierarchyPanel.lastSelectionDirWasUp = false;
								hierarchyEntry.SetSelected (!clickedHierarchyEntry.selected);
							}
						}
						else
						{
							int startIdx = lastEntryIdxHadSelectionSet - 1;
							int endIdx = idx;
							if (clickedHierarchyEntry.selected)
							{
								startIdx ++;
								endIdx ++;
							}
							else if (!hierarchyPanel.entries[startIdx].selected)
								startIdx ++;
							for (int i2 = startIdx; i2 >= endIdx; i2 --)
							{
								HierarchyEntry hierarchyEntry = hierarchyPanel.entries[i2];
								if (!hierarchyEntry.selected && !clickedHierarchyEntry.selected)
									HierarchyPanel.lastSelectionDirWasUp = true;
								hierarchyEntry.SetSelected (!clickedHierarchyEntry.selected);
							}
						}
					}
				}
				else if (Keyboard.current.leftCtrlKey.isPressed)
					clickedHierarchyEntry.SetSelected (!clickedHierarchyEntry.selected);
				else if (hierarchyPanel.selected.Length < 2 || !clickedHierarchyEntry.selected)
				{
					for (int i2 = 0; i2 < hierarchyPanel.selected.Length; i2 ++)
					{
						HierarchyEntry hierarchyEntry = hierarchyPanel.selected[i2];
						if (hierarchyEntry != clickedHierarchyEntry)
						{
							hierarchyEntry.SetSelected (false);
							i2 --;
						}
					}
					clickedHierarchyEntry.SetSelected (true);
				}
			}
		}

		public void OnMouseUp ()
		{
			if (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.leftCtrlKey.isPressed || HierarchyPanel.isDraggingEntry)
				return;
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				int idx = rectTrs.GetSiblingIndex();
				HierarchyEntry clickedHierarchyEntry = hierarchyPanel.entries[idx];
				if (hierarchyPanel.selected.Length < 2 || !clickedHierarchyEntry.selected)
					return;
				for (int i2 = 0; i2 < hierarchyPanel.selected.Length; i2 ++)
				{
					HierarchyEntry hierarchyEntry = hierarchyPanel.selected[i2];
					if (hierarchyEntry != clickedHierarchyEntry)
					{
						hierarchyEntry.SetSelected (false);
						i2 --;
					}
				}
				clickedHierarchyEntry.SetSelected (true);
			}
		}

		public void BeginDrag ()
		{
			HierarchyPanel.isDraggingEntry = true;
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
				hierarchyPanel.selected = hierarchyPanel.selected._Sort(new HierarchyEntryComparer());
				for (int i2 = 0; i2 < hierarchyPanel.selected.Length; i2 ++)
				{
					HierarchyEntry hierarchyEntry = hierarchyPanel.selected[i2];
					int idx = hierarchyEntry.rectTrs.GetSiblingIndex();
					if (idx > insertAt)
					{
						for (int i3 = hierarchyPanel.selected.Length - 1; i3 >= i2; i3 --)
						{
							hierarchyEntry = hierarchyPanel.selected[i3];
							idx = hierarchyEntry.rectTrs.GetSiblingIndex();
							hierarchyPanel.entries = hierarchyPanel.entries.RemoveAt(idx);
							hierarchyPanel.entries = hierarchyPanel.entries.Insert(hierarchyEntry, insertAt);
							hierarchyPanel.entriesParent.GetChild(idx).SetSiblingIndex(insertAt);
						}
						break;
					}
					else
					{
						hierarchyPanel.entries = hierarchyPanel.entries.RemoveAt(idx);
						hierarchyPanel.entries = hierarchyPanel.entries.Insert(hierarchyEntry, insertAt);
						hierarchyPanel.entriesParent.GetChild(idx).SetSiblingIndex(insertAt);
					}
				}
			}
			HierarchyPanel.isDraggingEntry = false;
		}

		void SetSelected (bool select)
		{
			if (select && !selected)
				hierarchyPanel.selected = hierarchyPanel.selected.Add(this);
			else if (!select && selected)
				hierarchyPanel.selected = hierarchyPanel.selected.Remove(this);
			HierarchyPanel.lastEntryIdxHadSelectionSet = rectTrs.GetSiblingIndex();
			selectedIndicator.enabled = select;
			selected = select;
		}

		public class HierarchyEntryComparer : IComparer<HierarchyEntry>
		{
			public int Compare (HierarchyEntry hierarchyEntry, HierarchyEntry hierarchyEntry2)
			{
				return MathfExtensions.Sign(hierarchyEntry.rectTrs.GetSiblingIndex() - hierarchyEntry2.rectTrs.GetSiblingIndex());
			}
		}
	}
}