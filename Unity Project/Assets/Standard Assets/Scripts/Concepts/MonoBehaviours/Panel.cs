using Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Frogger
{
	public class Panel : MonoBehaviour
	{
		public RectTransform rectTrs;
		public RectTransform tabRectTrs;
		public RectTransform contentsRectTrs;
		public RectTransform tabOptionsRectTrs;
		public Type type;
		public Canvas canvas;
		public TabOptionsUpdater tabOptionsUpdater;
		[HideInInspector]
		public bool mouseIsInside;
		static List<Panel> instances = new List<Panel>();
		static Vector2 SIZE_ON_BEGIN_DRAG = new Vector2(300, 300);
		Vector2 dragOffset;
		bool isDragging;
		DragUpdater dragUpdater;

		void Awake ()
		{
			instances.Add(this);
		}

		void OnDestroy ()
		{
			instances.Remove(this);
		}

		public void BeginDrag ()
		{
			isDragging = true;
			dragOffset = (Vector2) tabRectTrs.position - Mouse.current.position.ReadValue();
			if (tabOptionsUpdater != null)
				GameManager.updatables = GameManager.updatables.Remove(tabOptionsUpdater);
			dragUpdater = new DragUpdater(this);
			GameManager.updatables = GameManager.updatables.Add(dragUpdater);
		}

		public void Drag ()
		{
			if (!isDragging)
				return;
			tabRectTrs.position = dragOffset + Mouse.current.position.ReadValue();
		}

		public void EndDrag ()
		{
			isDragging = false;
			GameManager.updatables = GameManager.updatables.Remove(dragUpdater);
			if (mouseIsInside && tabOptionsUpdater == null)
				OnMouseEnter ();
		}

		public void OnMouseEnter ()
		{
			mouseIsInside = true;
			if (!isDragging)
			{
				tabOptionsUpdater = new TabOptionsUpdater(this);
				GameManager.updatables = GameManager.updatables.Add(tabOptionsUpdater);
			}
		}

		public void OnMouseExit ()
		{
			mouseIsInside = false;
			if (!tabOptionsRectTrs.gameObject.activeSelf)
			{
				GameManager.updatables = GameManager.updatables.Remove(tabOptionsUpdater);
				tabOptionsUpdater = null;
			}
		}

		public class DragUpdater : IUpdatable
		{
			public Panel panel;

			public DragUpdater (Panel panel)
			{
				this.panel = panel;
			}

			public void DoUpdate ()
			{
				if (Mouse.current.rightButton.wasPressedThisFrame)
				{
					panel.OnMouseEnter ();
					panel.EndDrag ();
				}
			}
		}

		public class TabOptionsUpdater : IUpdatable
		{
			public Panel panel;
			
			public TabOptionsUpdater (Panel panel)
			{
				this.panel = panel;
			}

			public void DoUpdate ()
			{
				if (panel.isDragging)
					return;
				if (Mouse.current.rightButton.wasPressedThisFrame)
				{
					panel.tabOptionsRectTrs.gameObject.SetActive(!panel.tabOptionsRectTrs.gameObject.activeSelf);
					if (!panel.mouseIsInside)
						GameManager.updatables = GameManager.updatables.Remove(panel.tabOptionsUpdater);
				}
				else if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					panel.tabOptionsRectTrs.gameObject.SetActive(false);
					if (!panel.mouseIsInside)
						GameManager.updatables = GameManager.updatables.Remove(panel.tabOptionsUpdater);
				}
			}
		}

		public enum Type
		{
			Scene,
			Hierarchy,
			Project,
			Inspector 
		}
	}
}