using Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

namespace Frogger
{
	public class Panel : MonoBehaviour
	{
		public RectTransform rectTrs;
		public RectTransform tabRectTrs;
		public RectTransform contentsRectTrs;
		public RectTransform tabOptionsRectTrs;
		public Type type;
		public TabOptionsUpdater tabOptionsUpdater;
		[HideInInspector]
		public bool mouseIsInsideTab;
		public RectTransform canvasRectTrs;
		public static List<Panel> instances = new List<Panel>();
		static Panel panelOfContentsMouseIsInside;
		Vector2 dragOffset;
		bool isDragging;
		DragUpdater dragUpdater;
		Vector2 sizeOnStartDrag;
		RectTransform contentsRectTrsCopy;

		void Awake ()
		{
			instances.Add(this);
		}
		
		void OnDestroy ()
		{
			instances.Remove(this);
		}

		void Start ()
		{
			Rect canvasWorldRect = canvasRectTrs.GetWorldRect();
			float leftHit = canvasWorldRect.xMin;
			float rightHit = canvasWorldRect.xMax;
			float bottHit = canvasWorldRect.yMin;
			float topHit = canvasWorldRect.yMax;
			Vector2 center = rectTrs.GetWorldRect().center;
			for (int i = 0; i < instances.Count; i ++)
			{
				Panel panel = instances[i];
				if (panel == this)
					continue;
				Rect panelRect = panel.rectTrs.GetWorldRect();
				if (center.x >= panelRect.center.x)
					leftHit = Mathf.Max(leftHit, panelRect.xMax);
				if (center.x <= panelRect.center.x)
					rightHit = Mathf.Min(rightHit, panelRect.xMin);
				if (center.y >= panelRect.center.y)
					bottHit = Mathf.Max(bottHit, panelRect.yMax);
				if (center.y <= panelRect.center.y)
					topHit = Mathf.Min(topHit, panelRect.yMin);
			}
			Rect worldRect = Rect.MinMaxRect(leftHit, bottHit, rightHit, topHit);
			RectTransform parentRectTrs = (RectTransform) rectTrs.parent;
			Vector3 localMin = parentRectTrs.InverseTransformPoint(new Vector3(worldRect.xMin, worldRect.yMin));
			Vector3 localMax = parentRectTrs.InverseTransformPoint(new Vector3(worldRect.xMax, worldRect.yMax));
			Vector2 localSize = (Vector2) (localMax - localMin);
			Vector2 localCenter = (Vector2) ((localMin + localMax) * 0.5f);
			rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Abs(localSize.x));
			rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(localSize.y));
			rectTrs.localPosition = localCenter;
			contentsRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsRectTrs.localPosition * 2;
		}

		public void BeginDrag ()
		{
			isDragging = true;
			dragOffset = (Vector2) tabRectTrs.position - Mouse.current.position.ReadValue();
			if (tabOptionsUpdater != null)
				GameManager.updatables = GameManager.updatables.Remove(tabOptionsUpdater);
			sizeOnStartDrag = contentsRectTrs.sizeDelta;
			contentsRectTrsCopy = Instantiate(contentsRectTrs, contentsRectTrs.parent);
			dragUpdater = new DragUpdater(this);
			GameManager.updatables = GameManager.updatables.Add(dragUpdater);
		}

		public void Drag ()
		{
			if (!isDragging)
				return;
			tabRectTrs.position = dragOffset + Mouse.current.position.ReadValue();
			if (panelOfContentsMouseIsInside == null)
				contentsRectTrsCopy.sizeDelta = sizeOnStartDrag;
			else if (panelOfContentsMouseIsInside != this)
			{
				Vector2 mousePos = Mouse.current.position.ReadValue();
				RectTransform contentsRectTrsMouseIsInside = panelOfContentsMouseIsInside.contentsRectTrs;
				Vector2 normalizedMousePosInContents = new Vector2(Mathf.InverseLerp(contentsRectTrsMouseIsInside.position.x - contentsRectTrsMouseIsInside.sizeDelta.x / 2, contentsRectTrsMouseIsInside.position.x + contentsRectTrsMouseIsInside.sizeDelta.x / 2, mousePos.x),
					Mathf.InverseLerp(contentsRectTrsMouseIsInside.position.y - contentsRectTrsMouseIsInside.sizeDelta.y / 2, contentsRectTrsMouseIsInside.position.y + contentsRectTrsMouseIsInside.sizeDelta.y / 2, mousePos.y));
				if (normalizedMousePosInContents.x < 1f / 3)
				{
					if (normalizedMousePosInContents.x < normalizedMousePosInContents.y)
					{
						
					}
					else
					{
						contentsRectTrsCopy.sizeDelta = new Vector2(contentsRectTrsMouseIsInside.sizeDelta.x / 2, contentsRectTrsMouseIsInside.sizeDelta.y);
						contentsRectTrsCopy.position = new Vector2(contentsRectTrsMouseIsInside.position.x + contentsRectTrsCopy.sizeDelta.x / 4, contentsRectTrsMouseIsInside.position.y);
					}
				}
				else if (normalizedMousePosInContents.x < 2f / 3)
				{
					
				}
				else
				{
					if (normalizedMousePosInContents.x > normalizedMousePosInContents.y)
					{
						
					}
					else
					{

					}
				}
			}
		}

		public void EndDrag ()
		{
			isDragging = false;
			GameManager.updatables = GameManager.updatables.Remove(dragUpdater);
			Destroy(contentsRectTrsCopy.gameObject);
			if (mouseIsInsideTab && tabOptionsUpdater == null)
				OnMouseEnterTab ();
		}

		public void OnMouseEnterTab ()
		{
			mouseIsInsideTab = true;
			if (!isDragging)
			{
				tabOptionsUpdater = new TabOptionsUpdater(this);
				GameManager.updatables = GameManager.updatables.Add(tabOptionsUpdater);
			}
		}

		public void OnMouseExitTab ()
		{
			mouseIsInsideTab = false;
			if (!tabOptionsRectTrs.gameObject.activeSelf)
			{
				GameManager.updatables = GameManager.updatables.Remove(tabOptionsUpdater);
				tabOptionsUpdater = null;
			}
		}

		public void OnMouseEnterContents ()
		{
			panelOfContentsMouseIsInside = this;
		}

		public void OnMouseExitContents ()
		{
			Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			for (int i = 0; i < instances.Count; i ++)
			{
				Panel panel = instances[i];
				if (panel == this)
					continue;
				Rect contentsWorldRect = panel.contentsRectTrs.GetWorldRect();
				if (contentsWorldRect.Contains(worldMousePos))
					return;
			}
			panelOfContentsMouseIsInside = null;
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
					panel.OnMouseEnterTab ();
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
					if (!panel.mouseIsInsideTab)
						GameManager.updatables = GameManager.updatables.Remove(panel.tabOptionsUpdater);
				}
				else if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					panel.tabOptionsRectTrs.gameObject.SetActive(false);
					if (!panel.mouseIsInsideTab)
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