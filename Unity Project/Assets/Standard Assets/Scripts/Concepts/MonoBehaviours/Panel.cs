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
			Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
			rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Abs(localSize.x));
			rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(localSize.y));
			rectTrs.localPosition = localMid;
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
			contentsRectTrsCopy.GetComponent<Canvas>().overrideSorting = true;
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
				Rect contentsWorldRectMouseIsIn = contentsRectTrsMouseIsIn.GetWorldRect();
				RectTransform previewParentRectTrs = (RectTransform) contentsRectTrsCopy.parent;
				Action joinLeft = () => { 
					Rect leftHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.xMin, contentsWorldRectMouseIsIn.yMin, contentsWorldRectMouseIsIn.center.x, contentsWorldRectMouseIsIn.yMax);
					Vector3 localMin = previewParentRectTrs.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMin, leftHalfWorldRect.yMin));
					Vector3 localMax = previewParentRectTrs.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMax, leftHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					contentsRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					contentsRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					contentsRectTrsCopy.localPosition = localMid; };
				Action joinRight = () => { 
					Rect rightHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.center.x, contentsWorldRectMouseIsIn.yMin, contentsWorldRectMouseIsIn.xMax, contentsWorldRectMouseIsIn.yMax);
					Vector3 localMin = previewParentRectTrs.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMin, rightHalfWorldRect.yMin));
					Vector3 localMax = previewParentRectTrs.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMax, rightHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					contentsRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					contentsRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					contentsRectTrsCopy.localPosition = localMid; };
				Action joinBott = () => { 
					Rect bottHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.xMin, contentsWorldRectMouseIsIn.yMin, contentsWorldRectMouseIsIn.xMax, contentsWorldRectMouseIsIn.center.y);
					Vector3 localMin = previewParentRectTrs.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMin, bottHalfWorldRect.yMin));
					Vector3 localMax = previewParentRectTrs.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMax, bottHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					contentsRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					contentsRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					contentsRectTrsCopy.localPosition = localMid; };
				Action joinTop = () => { 
					Rect topHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.xMin, contentsWorldRectMouseIsIn.center.y, contentsWorldRectMouseIsIn.xMax, contentsWorldRectMouseIsIn.yMax);
					Vector3 localMin = previewParentRectTrs.InverseTransformPoint(new Vector3(topHalfWorldRect.xMin, topHalfWorldRect.yMin));
					Vector3 localMax = previewParentRectTrs.InverseTransformPoint(new Vector3(topHalfWorldRect.xMax, topHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					contentsRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					contentsRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					contentsRectTrsCopy.localPosition = localMid; };
				Action swap = () => { contentsRectTrsCopy.position = contentsRectTrsMouseIsIn.position;
					contentsRectTrsCopy.sizeDelta = contentsRectTrsMouseIsIn.sizeDelta; };
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
				Rect contentsWorldRectMouseIsIn = contentsRectTrsMouseIsIn.GetWorldRect();
				Transform parentRectTrs = rectTrs.parent;
				RectTransform panelRectTrsMouseIsIn = panelOfContentsMouseIsIn.rectTrs;
				Transform parentOfPanelMouseIsIn = panelRectTrsMouseIsIn.parent;
				Action joinLeft = () => { 
					Rect leftHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.xMin, contentsWorldRectMouseIsIn.yMin, contentsWorldRectMouseIsIn.center.x, contentsWorldRectMouseIsIn.yMax);
					Vector3 localMin = parentRectTrs.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMin, leftHalfWorldRect.yMin));
					Vector3 localMax = parentRectTrs.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMax, leftHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					rectTrs.localPosition = localMid;
					contentsRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsRectTrs.localPosition * 2;
					
					Rect rightHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.center.x, contentsWorldRectMouseIsIn.yMin, contentsWorldRectMouseIsIn.xMax, contentsWorldRectMouseIsIn.yMax);
					Vector3 panelContentsLocalMin = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMin, rightHalfWorldRect.yMin));
					Vector3 panelContentsLocalMax = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMax, rightHalfWorldRect.yMax));
					Vector2 panelContentsLocalSize = (Vector2) (panelContentsLocalMax - panelContentsLocalMin);
					Vector2 panelContentsLocalMid = (Vector2) ((panelContentsLocalMin + panelContentsLocalMax) / 2);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelContentsLocalSize.x);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelContentsLocalSize.y);
					panelRectTrsMouseIsIn.localPosition = panelContentsLocalMid;
					contentsRectTrsMouseIsIn.sizeDelta = panelRectTrsMouseIsIn.sizeDelta + (Vector2) contentsRectTrsMouseIsIn.localPosition * 2;
				};
				Action joinRight = () => { 
					Rect rightHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.center.x, contentsWorldRectMouseIsIn.yMin, contentsWorldRectMouseIsIn.xMax, contentsWorldRectMouseIsIn.yMax);
					Vector3 localMin = parentRectTrs.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMin, rightHalfWorldRect.yMin));
					Vector3 localMax = parentRectTrs.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMax, rightHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					rectTrs.localPosition = localMid;
					contentsRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsRectTrs.localPosition * 2;
					
					Rect leftHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.xMin, contentsWorldRectMouseIsIn.yMin, contentsWorldRectMouseIsIn.center.x, contentsWorldRectMouseIsIn.yMax);
					Vector3 panelContentsLocalMin = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMin, leftHalfWorldRect.yMin));
					Vector3 panelContentsLocalMax = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMax, leftHalfWorldRect.yMax));
					Vector2 panelContentsLocalSize = (Vector2) (panelContentsLocalMax - panelContentsLocalMin);
					Vector2 panelContentsLocalMid = (Vector2) ((panelContentsLocalMin + panelContentsLocalMax) / 2);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelContentsLocalSize.x);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelContentsLocalSize.y);
					panelRectTrsMouseIsIn.localPosition = panelContentsLocalMid;
					contentsRectTrsMouseIsIn.sizeDelta = panelRectTrsMouseIsIn.sizeDelta + (Vector2) contentsRectTrsMouseIsIn.localPosition * 2;
				};
				Action joinBott = () => { 
					Rect bottHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.xMin, contentsWorldRectMouseIsIn.yMin, contentsWorldRectMouseIsIn.xMax, contentsWorldRectMouseIsIn.center.y);
					Vector3 localMin = parentRectTrs.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMin, bottHalfWorldRect.yMin));
					Vector3 localMax = parentRectTrs.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMax, bottHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					rectTrs.localPosition = localMid;
					contentsRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsRectTrs.localPosition * 2;
					
					Rect topHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.xMin, contentsWorldRectMouseIsIn.center.y, contentsWorldRectMouseIsIn.xMax, contentsWorldRectMouseIsIn.yMax);
					Vector3 panelContentsLocalMin = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(topHalfWorldRect.xMin, topHalfWorldRect.yMin));
					Vector3 panelContentsLocalMax = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(topHalfWorldRect.xMax, topHalfWorldRect.yMax));
					Vector2 panelContentsLocalSize = (Vector2) (panelContentsLocalMax - panelContentsLocalMin);
					Vector2 panelContentsLocalMid = (Vector2) ((panelContentsLocalMin + panelContentsLocalMax) / 2);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelContentsLocalSize.x);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelContentsLocalSize.y);
					panelRectTrsMouseIsIn.localPosition = panelContentsLocalMid;
					contentsRectTrsMouseIsIn.sizeDelta = panelRectTrsMouseIsIn.sizeDelta + (Vector2) contentsRectTrsMouseIsIn.localPosition * 2;
				};
				Action joinTop = () => { 
					Rect topHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.xMin, contentsWorldRectMouseIsIn.center.y, contentsWorldRectMouseIsIn.xMax, contentsWorldRectMouseIsIn.yMax);
					Vector3 localMin = parentRectTrs.InverseTransformPoint(new Vector3(topHalfWorldRect.xMin, topHalfWorldRect.yMin));
					Vector3 localMax = parentRectTrs.InverseTransformPoint(new Vector3(topHalfWorldRect.xMax, topHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					rectTrs.localPosition = localMid;
					contentsRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsRectTrs.localPosition * 2;
					
					Rect bottHalfWorldRect = Rect.MinMaxRect(contentsWorldRectMouseIsIn.xMin, contentsWorldRectMouseIsIn.yMin, contentsWorldRectMouseIsIn.xMax, contentsWorldRectMouseIsIn.center.y);
					Vector3 panelContentsLocalMin = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMin, bottHalfWorldRect.yMin));
					Vector3 panelContentsLocalMax = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMax, bottHalfWorldRect.yMax));
					Vector2 panelContentsLocalSize = (Vector2) (panelContentsLocalMax - panelContentsLocalMin);
					Vector2 panelContentsLocalMid = (Vector2) ((panelContentsLocalMin + panelContentsLocalMax) / 2);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelContentsLocalSize.x);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelContentsLocalSize.y);
					panelRectTrsMouseIsIn.localPosition = panelContentsLocalMid;
					contentsRectTrsMouseIsIn.sizeDelta = panelRectTrsMouseIsIn.sizeDelta + (Vector2) contentsRectTrsMouseIsIn.localPosition * 2;
				};
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
			Vector2 worldMousePos = Mouse.current.position.ReadValue();
			for (int i = 0; i < instances.Count; i ++)
			{
				Panel panel = instances[i];
				Rect contentsWorldRect = panel.contentsRectTrs.GetWorldRect();
				if (contentsWorldRect.Contains(worldMousePos))
				{
					panelOfContentsMouseIsIn = panel;
					return;
				}
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