using TMPro;
using System;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace EternityEngine
{
	public class HierarchyEntry : MonoBehaviour, IUpdatable
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
		public TMP_InputField nameInputField;
		public RectTransform optionsRectTrs;
		OptionsUpdater optionsUpdater;
		int insertAt;

		public void OnMouseEnter ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void OnMouseExit ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public void DoUpdate ()
		{
			if (Mouse.current.rightButton.wasPressedThisFrame)
			{
				SetSelected (true);
				ToggleOptions ();
			}
		}

		public void OnMouseDown ()
		{
			int lastEntryIdxHadSelectionSet = HierarchyPanel.lastEntryIdxHadSelectionSet;
			int prevSelectedCnt = 0;
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				prevSelectedCnt = hierarchyPanel.selected.Length;;
				int idx = rectTrs.GetSiblingIndex();
				HierarchyEntry clickedHierarchyEntry = hierarchyPanel.entries[idx];
				if (Keyboard.current.leftShiftKey.isPressed)
				{
					if (lastEntryIdxHadSelectionSet == -1)
						clickedHierarchyEntry.SetSelected (true);
					else
					{
						if (idx > lastEntryIdxHadSelectionSet)
						{
							int startIdx = lastEntryIdxHadSelectionSet;
							int endIdx = idx;
							if (hierarchyPanel.selected.Length == 1)
								startIdx ++;
							else if (clickedHierarchyEntry.selected)
								endIdx --;
							for (int i2 = startIdx; i2 <= endIdx; i2 ++)
							{
								HierarchyEntry hierarchyEntry = hierarchyPanel.entries[i2];
								hierarchyEntry.SetSelected (!clickedHierarchyEntry.selected);
							}
						}
						else
						{
							int startIdx = lastEntryIdxHadSelectionSet;
							int endIdx = idx;
							if (hierarchyPanel.selected.Length == 1)
								startIdx --;
							else if (clickedHierarchyEntry.selected)
								endIdx ++;
							for (int i2 = startIdx; i2 >= endIdx; i2 --)
							{
								HierarchyEntry hierarchyEntry = hierarchyPanel.entries[i2];
								hierarchyEntry.SetSelected (!clickedHierarchyEntry.selected);
							}
						}
						HierarchyPanel.lastEntryIdxHadSelectionSet = idx;
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
			InspectorPanel.RegenEntries (prevSelectedCnt > 1);
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
			InspectorPanel.RegenEntries (true);
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
				hierarchyPanel.selected = hierarchyPanel.selected._Sort(new Comparer());
				for (int i2 = 0; i2 < hierarchyPanel.selected.Length; i2 ++)
				{
					HierarchyEntry hierarchyEntry = hierarchyPanel.selected[i2];
					int idx = hierarchyEntry.rectTrs.GetSiblingIndex();
					if (idx > insertAt)
					{
						for (int i3 = hierarchyPanel.selected.Length - 1; i3 >= i2; i3 --)
						{
							hierarchyEntry = hierarchyPanel.selected[i3];
							hierarchyEntry.Reorder (insertAt);
						}
						break;
					}
					else
						hierarchyEntry.Reorder (insertAt);
				}
			}
			HierarchyPanel.isDraggingEntry = false;
		}

		public void SetSelected (bool select)
		{
			if (select && !selected)
				hierarchyPanel.selected = hierarchyPanel.selected.Add(this);
			else if (!select && selected)
				hierarchyPanel.selected = hierarchyPanel.selected.Remove(this);
			HierarchyPanel.lastEntryIdxHadSelectionSet = rectTrs.GetSiblingIndex();
			selectedIndicator.enabled = select;
			selected = select;
		}

		public void OnChangedNameInputField (string name)
		{
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				HierarchyEntry[] selected = hierarchyPanel.selected;
				for (int i2 = 0; i2 < selected.Length; i2 ++)
				{
					HierarchyEntry hierarchyEntry = selected[i2];
					hierarchyEntry.nameInputField.text = name;
				}
			}
		}

		public void SetName (string name)
		{
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				HierarchyEntry[] selected = hierarchyPanel.selected;
				selected = selected._Sort(new Comparer());
				for (int i2 = 0; i2 < selected.Length; i2 ++)
				{
					HierarchyEntry hierarchyEntry = selected[i2];
					if (i == 0)
						hierarchyEntry.ob.name = EternityEngine.GetUniqueName(name, hierarchyEntry.ob);
					hierarchyEntry.nameText.text = hierarchyEntry.ob.name;
					hierarchyEntry.nameInputField.gameObject.SetActive(false);
				}
			}
		}

		public void Reorder (int insertAt)
		{
			int idx = rectTrs.GetSiblingIndex();
			hierarchyPanel.entries = hierarchyPanel.entries.RemoveAt(idx);
			hierarchyPanel.entries = hierarchyPanel.entries.Insert(this, insertAt);
			hierarchyPanel.entriesParent.GetChild(idx).SetSiblingIndex(insertAt);
			if (HierarchyPanel.lastEntryIdxHadSelectionSet == idx)
				HierarchyPanel.lastEntryIdxHadSelectionSet = insertAt;
		}

		public void ToggleOptions ()
		{
			optionsRectTrs.gameObject.SetActive(!optionsRectTrs.gameObject.activeSelf);
			if (optionsRectTrs.gameObject.activeSelf)
			{
				optionsUpdater = new OptionsUpdater(this);
				GameManager.updatables = GameManager.updatables.Add(optionsUpdater);
			}
			else
				GameManager.updatables = GameManager.updatables.Add(optionsUpdater);
		}

		class OptionsUpdater : IUpdatable
		{
			HierarchyEntry hierarchyEntry;

			public OptionsUpdater (HierarchyEntry hierarchyEntry)
			{
				this.hierarchyEntry = hierarchyEntry;
			}

			public void DoUpdate ()
			{
				if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
				{
					Vector2 mousePos = Mouse.current.position.ReadValue();
					Rect optionssWorldRect = hierarchyEntry.optionsRectTrs.GetWorldRect();
					if (!optionssWorldRect.Contains(mousePos))
					{
						hierarchyEntry.optionsRectTrs.gameObject.SetActive(false);
						GameManager.updatables = GameManager.updatables.Remove(this);
					}
				}
			}
		}

		public class Comparer : IComparer<HierarchyEntry>
		{
			public int Compare (HierarchyEntry hierarchyEntry, HierarchyEntry hierarchyEntry2)
			{
				return MathfExtensions.Sign(hierarchyEntry.rectTrs.GetSiblingIndex() - hierarchyEntry2.rectTrs.GetSiblingIndex());
			}
		}
	}
}