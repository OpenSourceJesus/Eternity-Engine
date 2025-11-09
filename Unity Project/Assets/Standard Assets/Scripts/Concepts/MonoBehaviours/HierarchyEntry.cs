using TMPro;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

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
			hierarchyPanel.insertionIndicator = Instantiate(EternityEngine.instance.insertionIndicatorPrefab);
		}

		public void Drag ()
		{
			
		}

		public void EndDrag ()
		{
			Destroy(hierarchyPanel.insertionIndicator.gameObject);
			
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