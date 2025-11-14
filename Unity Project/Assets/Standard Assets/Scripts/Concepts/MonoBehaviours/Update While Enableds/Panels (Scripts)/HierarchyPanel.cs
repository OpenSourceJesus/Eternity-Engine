using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace EternityEngine
{
	public class HierarchyPanel : Panel
	{
		public RectTransform entriesParent;
		[HideInInspector]
		public HierarchyEntry[] entries = new HierarchyEntry[0];
		[HideInInspector]
		public HierarchyEntry[] selected = new HierarchyEntry[0];
		[HideInInspector]
		public Image insertionIndicator;
		public RectTransform addPresetObjectOptionsRectTrs;
		public RectTransform addPresetObjectButtonRectTrs;
		public static int lastEntryIdxHadSelectionSet = -1;
		public static bool isDraggingEntry;
		public new static HierarchyPanel[] instances = new HierarchyPanel[0];

		public override void Awake ()
		{
			base.Awake ();
			instances = instances.Add(this);
		}
		
		public override void OnDestroy ()
		{
			base.OnDestroy ();
			instances = instances.Remove(this);
		}

		public void ToggleAddPresetObjectOptions ()
		{
			addPresetObjectOptionsRectTrs.gameObject.SetActive(!addPresetObjectOptionsRectTrs.gameObject.activeSelf);
			GameManager.updatables = GameManager.updatables.Add(new AddPresetObjectOptionsUpdater(this));
		}

		public void DeselectAll ()
		{
			int prevSelectedCnt = 0;
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				prevSelectedCnt = hierarchyPanel.selected.Length;
				for (int i2 = 0; i2 < hierarchyPanel.selected.Length; i2 ++)
				{
					HierarchyEntry entry = hierarchyPanel.selected[i2];
					entry.SetSelected (false);
					i2 --;
				}
			}
			InspectorPanel.ClearEntries (prevSelectedCnt > 1);
		}

		class AddPresetObjectOptionsUpdater : IUpdatable
		{
			public HierarchyPanel hierarchyPanel;

			public AddPresetObjectOptionsUpdater (HierarchyPanel hierarchyPanel)
			{
				this.hierarchyPanel = hierarchyPanel;
			}

			public void DoUpdate ()
			{
				if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
				{
					Vector2 mousePos = Mouse.current.position.ReadValue();
					Rect addPresetObjectOptionsWorldRect = hierarchyPanel.addPresetObjectOptionsRectTrs.GetWorldRect();
					if (!addPresetObjectOptionsWorldRect.Contains(mousePos))
					{
						Rect addPresetObjectButtonWorldRect = hierarchyPanel.addPresetObjectButtonRectTrs.GetWorldRect();
						if (!addPresetObjectButtonWorldRect.Contains(mousePos))
							hierarchyPanel.addPresetObjectOptionsRectTrs.gameObject.SetActive(false);
						GameManager.updatables = GameManager.updatables.Remove(this);
					}
				}
			}
		}
	}
}