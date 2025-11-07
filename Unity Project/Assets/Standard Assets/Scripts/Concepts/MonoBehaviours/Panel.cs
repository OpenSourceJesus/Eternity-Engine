using System;
using Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
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
		public TabOptionsUpdater tabOptionsUpdater;
		[HideInInspector]
		public bool mouseIsInTab;
		public RectTransform canvasRectTrs;
		public static List<Panel> instances = new List<Panel>();
		static Panel panelOfContentsMouseIsIn;
		Vector2 offDrag;
		bool isDragging;
		DragUpdater dragUpdater;
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
			offDrag = (Vector2) tabRectTrs.position - Mouse.current.position.ReadValue();
			if (tabOptionsUpdater != null)
				GameManager.updatables = GameManager.updatables.Remove(tabOptionsUpdater);
			contentsRectTrsCopy = Instantiate(contentsRectTrs, contentsRectTrs.parent);
			Destroy(contentsRectTrsCopy.GetComponent<EventTrigger>());
			dragUpdater = new DragUpdater(this);
			GameManager.updatables = GameManager.updatables.Add(dragUpdater);
		}

		public void Drag ()
		{
			if (!isDragging)
				return;
			tabRectTrs.position = offDrag + Mouse.current.position.ReadValue();
			if (panelOfContentsMouseIsIn == null || panelOfContentsMouseIsIn == this)
			{
				contentsRectTrsCopy.position = contentsRectTrs.position;
				contentsRectTrsCopy.sizeDelta = contentsRectTrs.sizeDelta;
			}
			else if (panelOfContentsMouseIsIn != this)
			{
				RectTransform contentsRectTrsMouseIsIn = panelOfContentsMouseIsIn.contentsRectTrs;
				Action joinLeft = () => { contentsRectTrsCopy.sizeDelta = new Vector2(contentsRectTrsMouseIsIn.sizeDelta.x / 2, contentsRectTrsMouseIsIn.sizeDelta.y);
					contentsRectTrsCopy.position = new Vector2(contentsRectTrsMouseIsIn.position.x - contentsRectTrsMouseIsIn.sizeDelta.x / 4, contentsRectTrsMouseIsIn.position.y);
					print("joinLeft"); };
				Action joinRight = () => { contentsRectTrsCopy.sizeDelta = new Vector2(contentsRectTrsMouseIsIn.sizeDelta.x / 2, contentsRectTrsMouseIsIn.sizeDelta.y);
					contentsRectTrsCopy.position = new Vector2(contentsRectTrsMouseIsIn.position.x + contentsRectTrsMouseIsIn.sizeDelta.x / 4, contentsRectTrsMouseIsIn.position.y);
					print("joinRight"); };
				Action joinBott = () => { contentsRectTrsCopy.sizeDelta = new Vector2(contentsRectTrsMouseIsIn.sizeDelta.x, contentsRectTrsMouseIsIn.sizeDelta.y / 2);
					contentsRectTrsCopy.position = new Vector2(contentsRectTrsMouseIsIn.position.x, contentsRectTrsMouseIsIn.position.y - contentsRectTrsMouseIsIn.sizeDelta.y / 4);
					print("joinBott"); };
				Action joinTop = () => { contentsRectTrsCopy.sizeDelta = new Vector2(contentsRectTrsMouseIsIn.sizeDelta.x, contentsRectTrsMouseIsIn.sizeDelta.y / 2);
					contentsRectTrsCopy.position = new Vector2(contentsRectTrsMouseIsIn.position.x, contentsRectTrsMouseIsIn.position.y + contentsRectTrsMouseIsIn.sizeDelta.y / 4);
					print("joinTop"); };
				Action swap = () => { contentsRectTrsCopy.position = contentsRectTrsMouseIsIn.position;
					contentsRectTrsCopy.sizeDelta = contentsRectTrsMouseIsIn.sizeDelta;
					print("swap"); };
				JoinOrSwapPanels (panelOfContentsMouseIsIn, joinLeft, joinRight, joinBott, joinTop, swap);
			}
		}

		public void EndDrag ()
		{
			isDragging = false;
			GameManager.updatables = GameManager.updatables.Remove(dragUpdater);
			if (panelOfContentsMouseIsIn != null && panelOfContentsMouseIsIn != this)
			{
				RectTransform contentsRectTrsMouseIsIn = panelOfContentsMouseIsIn.contentsRectTrs;
				Action joinLeft = () => { rectTrs.sizeDelta = new Vector2(contentsRectTrsMouseIsIn.sizeDelta.x / 2, contentsRectTrsMouseIsIn.sizeDelta.y);
					rectTrs.position = new Vector2(contentsRectTrsMouseIsIn.position.x - contentsRectTrsMouseIsIn.sizeDelta.x / 4, contentsRectTrsMouseIsIn.position.y); };
				Action joinRight = () => { rectTrs.sizeDelta = new Vector2(contentsRectTrsMouseIsIn.sizeDelta.x / 2, contentsRectTrsMouseIsIn.sizeDelta.y);
					rectTrs.position = new Vector2(contentsRectTrsMouseIsIn.position.x + contentsRectTrsMouseIsIn.sizeDelta.x / 4, contentsRectTrsMouseIsIn.position.y); };
				Action joinBott = () => { rectTrs.sizeDelta = new Vector2(contentsRectTrsMouseIsIn.sizeDelta.x, contentsRectTrsMouseIsIn.sizeDelta.y / 2);
					rectTrs.position = new Vector2(contentsRectTrsMouseIsIn.position.x, contentsRectTrsMouseIsIn.position.y - contentsRectTrsMouseIsIn.sizeDelta.y / 4); };
				Action joinTop = () => { rectTrs.sizeDelta = new Vector2(contentsRectTrsMouseIsIn.sizeDelta.x, contentsRectTrsMouseIsIn.sizeDelta.y / 2);
					rectTrs.position = new Vector2(contentsRectTrsMouseIsIn.position.x, contentsRectTrsMouseIsIn.position.y + contentsRectTrsMouseIsIn.sizeDelta.y / 4); };
				Action swap = () => { Vector2 prevPos = rectTrs.position;
					Vector2 prevSize = rectTrs.sizeDelta;
					rectTrs.position = panelOfContentsMouseIsIn.rectTrs.position;
					rectTrs.sizeDelta = panelOfContentsMouseIsIn.rectTrs.sizeDelta;
					panelOfContentsMouseIsIn.rectTrs.position = prevPos;
					panelOfContentsMouseIsIn.rectTrs.sizeDelta = prevSize; };
				JoinOrSwapPanels (panelOfContentsMouseIsIn, joinLeft, joinRight, joinBott, joinTop, swap);
			}
			if (contentsRectTrsCopy != null)
				Destroy(contentsRectTrsCopy.gameObject);
			if (mouseIsInTab && tabOptionsUpdater == null)
				OnMouseEnterTab ();
		}

		void JoinOrSwapPanels (Panel panel, Action joinLeft, Action joinRight, Action joinBott, Action joinTop, Action swap)
		{
			Vector2 mousePos = Mouse.current.position.ReadValue();
			RectTransform panelContentsRectTrs = panel.contentsRectTrs;
			Vector2 normalizedMousePosInContents = new Vector2(Mathf.InverseLerp(panelContentsRectTrs.position.x - panelContentsRectTrs.sizeDelta.x / 2, panelContentsRectTrs.position.x + panelContentsRectTrs.sizeDelta.x / 2, mousePos.x),
				Mathf.InverseLerp(panelContentsRectTrs.position.y - panelContentsRectTrs.sizeDelta.y / 2, panelContentsRectTrs.position.y + panelContentsRectTrs.sizeDelta.y / 2, mousePos.y));
			if (normalizedMousePosInContents.x < 1f / 3)
			{
				if (normalizedMousePosInContents.y < 1f / 3)
				{
					if (normalizedMousePosInContents.x < normalizedMousePosInContents.y)
						joinLeft ();
					else
						joinBott ();
				}
				else if (normalizedMousePosInContents.y > 2f / 3)
				{
					if (normalizedMousePosInContents.x < 1f - normalizedMousePosInContents.y)
						joinLeft ();
					else
						joinTop ();
				}
				else
					joinLeft ();
			}
			else if (normalizedMousePosInContents.x < 2f / 3)
			{
				if (normalizedMousePosInContents.y < 1f / 3)
					joinBott ();
				else if (normalizedMousePosInContents.y < 2f / 3)
					swap ();
				else
					joinTop ();
			}
			else if (normalizedMousePosInContents.y < 1f / 3)
			{
				if (1f - normalizedMousePosInContents.x < normalizedMousePosInContents.y)
					joinBott ();
				else
					joinRight ();
			}
			else if (normalizedMousePosInContents.y < 2f / 3)
			{
				if (normalizedMousePosInContents.x < 1f / 3)
					joinLeft ();
				else if (normalizedMousePosInContents.x < 2f / 3)
					swap ();
				else
					joinRight ();
			}
			else
			{
				if (normalizedMousePosInContents.x < normalizedMousePosInContents.y)
					joinTop ();
				else
					joinRight ();
			}
		}

		public void OnMouseEnterTab ()
		{
			mouseIsInTab = true;
			if (!isDragging)
			{
				tabOptionsUpdater = new TabOptionsUpdater(this);
				GameManager.updatables = GameManager.updatables.Add(tabOptionsUpdater);
			}
		}

		public void OnMouseExitTab ()
		{
			mouseIsInTab = false;
			if (!tabOptionsRectTrs.gameObject.activeSelf)
			{
				GameManager.updatables = GameManager.updatables.Remove(tabOptionsUpdater);
				tabOptionsUpdater = null;
			}
		}

		public void OnMouseEnterContents ()
		{
			panelOfContentsMouseIsIn = this;
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
			panelOfContentsMouseIsIn = null;
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
					if (!panel.mouseIsInTab)
						GameManager.updatables = GameManager.updatables.Remove(panel.tabOptionsUpdater);
				}
				else if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					panel.tabOptionsRectTrs.gameObject.SetActive(false);
					if (!panel.mouseIsInTab)
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