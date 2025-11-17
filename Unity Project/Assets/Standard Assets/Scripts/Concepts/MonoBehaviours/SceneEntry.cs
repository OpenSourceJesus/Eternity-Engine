using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class SceneEntry : MonoBehaviour
	{
		public RectTransform rectTrs;
		public Image img;
		[HideInInspector]
		public ScenePanel scenePanel;
		public GameObject selectedIndicatorGo;
		[HideInInspector]
		public HierarchyEntry[] hierarchyEntries = new HierarchyEntry[0];
		[HideInInspector]
		public bool selected;

		void Start ()
		{
			if (hierarchyEntries[0].selected)
				SetSelected (true);
		}

		public void SetSelected (bool select)
		{
			if (select && !selected)
				scenePanel.selected = scenePanel.selected.Add(this);
			else if (!select && selected)
				scenePanel.selected = scenePanel.selected.Remove(this);
			int idx = rectTrs.GetSiblingIndex();
			for (int i = 0; i < ScenePanel.instances.Length; i ++)
			{
				ScenePanel scenePanel = ScenePanel.instances[i];
				SceneEntry entry = scenePanel.entries[idx];
				entry.selectedIndicatorGo.SetActive(select);
				entry.selected = select;
				for (int i2 = 0; i2 < entry.hierarchyEntries.Length; i2 ++)
				{
					HierarchyEntry hierarchyEntry = entry.hierarchyEntries[i2];
					if (hierarchyEntry.selected != select)
						hierarchyEntry.SetSelected (select);
				}
			}
		}
	}
}