using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace EternityEngine
{
	public class InspectorPanel : Panel
	{
		public RectTransform entriesParent;
		[HideInInspector]
		public InspectorEntry[] entries = new InspectorEntry[0];
		[HideInInspector]
		public Image insertionIndicator;
		public RectTransform addComponentOptionsRectTrs;
		public RectTransform addComponentButtonRectTrs;
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
					inspectorPanel.entriesParent.GetChild(i2).gameObject.SetActive(false);
				inspectorPanel.entries = new InspectorEntry[0];
			}
		}

		public static InspectorEntry[] AddEntries (_Component component)
		{
			InspectorEntry[] output = new InspectorEntry[instances.Length];
			for (int i = 0; i < instances.Length; i ++)
			{
				InspectorPanel inspectorPanel = instances[i];
				InspectorEntry inspectorEntry = null;
				if (component.inspectorEntries.Length <= i)
				{
					inspectorEntry = Instantiate(component.inspectorEntryPrefab, inspectorPanel.entriesParent);
					inspectorEntry.component = component;
					inspectorEntry.inspectorPanel = inspectorPanel;
					for (int i2 = 0; i2 < component.floatValues.Length; i2 ++)
					{
						FloatValue floatValue = component.floatValues[i2];
						inspectorEntry.floatValuesEntries[i2].value = floatValue;
					}
					for (int i2 = 0; i2 < component.vector3Values.Length; i2 ++)
					{
						Vector3Value vector3Value = component.vector3Values[i2];
						inspectorEntry.vector3ValuesEntries[i2].value = vector3Value;
					}
					if (component.collapsed)
						inspectorEntry.SetCollapsed (true);
					component.inspectorEntries = component.inspectorEntries.Add(inspectorEntry);
				}
				else
				{
					inspectorEntry = component.inspectorEntries[i];
					inspectorEntry.gameObject.SetActive(true);
				}
				inspectorPanel.entries = inspectorPanel.entries.Add(inspectorEntry);
				output[i] = inspectorEntry;
			}
			return output;
		}

		public void ToggleAddComponentOptions ()
		{
			addComponentOptionsRectTrs.gameObject.SetActive(!addComponentOptionsRectTrs.gameObject.activeSelf);
			GameManager.updatables = GameManager.updatables.Add(new AddComponentOptionsUpdater(this));
		}

		public class AddComponentOptionsUpdater : IUpdatable
		{
			public InspectorPanel inspectorPanel;

			public AddComponentOptionsUpdater (InspectorPanel inspectorPanel)
			{
				this.inspectorPanel = inspectorPanel;
			}

			public void DoUpdate ()
			{
				if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
				{
					Vector2 mousePos = Mouse.current.position.ReadValue();
					Rect addComponentOptionsWorldRect = inspectorPanel.addComponentOptionsRectTrs.GetWorldRect();
					if (!addComponentOptionsWorldRect.Contains(mousePos))
					{
						Rect addComponentButtonWorldRect = inspectorPanel.addComponentButtonRectTrs.GetWorldRect();
						if (!addComponentButtonWorldRect.Contains(mousePos))
							inspectorPanel.addComponentOptionsRectTrs.gameObject.SetActive(false);
						GameManager.updatables = GameManager.updatables.Remove(this);
					}
				}
			}
		}
	}
}