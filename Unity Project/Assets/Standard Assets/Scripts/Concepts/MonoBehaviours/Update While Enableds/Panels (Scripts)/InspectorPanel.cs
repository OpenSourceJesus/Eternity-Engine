using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class InspectorPanel : Panel
	{
		public RectTransform entriesParent;
		[HideInInspector]
		public InspectorEntry[] entries = new InspectorEntry[0];
		[HideInInspector]
		public Image insertionIndicator;
		public static bool isDraggingEntry;
		public new static InspectorPanel[] instances = new InspectorPanel[0];

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

		public static void RegenEntries ()
		{
			ClearEntries ();
			HierarchyEntry[] selected = HierarchyPanel.instances[0].selected;
			for (int i = 0; i < selected.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selected[i];
				_Component[] components = hierarchyEntry.ob.components;
				for (int i2 = 0; i2 < components.Length; i2 ++)
				{
					_Component component = components[i2];
					AddEntries (component);
				}
			}
		}

		public static void ClearEntries ()
		{
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				for (int i2 = 0; i2 < inspectorPanel.entriesParent.childCount; i2 ++)
					Destroy(inspectorPanel.entriesParent.GetChild(i2).gameObject);
				inspectorPanel.entries = new InspectorEntry[0];
			}
		}

		public static void AddEntries (_Component component)
		{
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				InspectorEntry inspectorEntry = Instantiate(component.inspectorEntryPrefab, inspectorPanel.entriesParent);
				inspectorEntry.component = component;
				inspectorEntry.inspectorPanel = inspectorPanel;
				inspectorPanel.entries = inspectorPanel.entries.Add(inspectorEntry);
			}
		}
	}
}