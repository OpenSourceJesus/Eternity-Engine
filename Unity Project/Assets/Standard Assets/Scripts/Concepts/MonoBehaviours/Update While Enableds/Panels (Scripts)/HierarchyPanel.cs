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
		public static int lastEntryIdxHadSelectionSet;
		public static bool lastSelectionDirWasUp;
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
	}
}