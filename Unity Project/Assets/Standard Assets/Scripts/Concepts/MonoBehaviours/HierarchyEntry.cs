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
				if (!Keyboard.current.leftShiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
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
					if (!clickedHierarchyEntry.selected)
						clickedHierarchyEntry.SetSelected (true);
				}
				else if (Keyboard.current.leftShiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
				{
					for (int i2 = idx + 1; i2 < hierarchyPanel.entries.Length; i2 ++)
					{
						HierarchyEntry hierarchyEntry = hierarchyPanel.entries[i2];
						if (hierarchyEntry.selected)
						{
							for (int i3 = idx + 1; i3 < i2; i3 ++)
							{
								HierarchyEntry _hierarchyEntry = hierarchyPanel.entries[i3];
								_hierarchyEntry.SetSelected (true);
							}
							break;
						}
					}
					for (int i2 = idx - 1; i2 >= 0; i2 --)
					{
						HierarchyEntry hierarchyEntry = hierarchyPanel.entries[i2];
						if (hierarchyEntry.selected)
						{
							for (int i3 = idx - 1; i3 > i2; i3 --)
							{
								HierarchyEntry _hierarchyEntry = hierarchyPanel.entries[i3];
								_hierarchyEntry.SetSelected (true);
							}
							break;
						}
					}
					if (!clickedHierarchyEntry.selected)
						clickedHierarchyEntry.SetSelected (true);
				}
				else if (!Keyboard.current.leftShiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
					clickedHierarchyEntry.SetSelected (!clickedHierarchyEntry.selected);
				else if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
				{
					
				}
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
				int idx = rectTrs.GetSiblingIndex();
				HierarchyEntry hierarchyEntry = hierarchyPanel.entries[idx];
				hierarchyPanel.entries = hierarchyPanel.entries.RemoveAt(idx);
				hierarchyPanel.entries = hierarchyPanel.entries.Insert(hierarchyEntry, insertAt);
				hierarchyPanel.entriesParent.GetChild(idx).SetSiblingIndex(insertAt);
			}
		}

		void SetSelected (bool select)
		{
			if (select && !selected)
			{
				hierarchyPanel.selected = hierarchyPanel.selected.Add(this);
				hierarchyPanel.lastEntryChangedSelection = this;
			}
			else if (!select && selected)
			{
				hierarchyPanel.selected = hierarchyPanel.selected.Remove(this);
				hierarchyPanel.lastEntryChangedSelection = this;
			}
			selectedIndicator.enabled = select;
			selected = select;
		}
	}
}