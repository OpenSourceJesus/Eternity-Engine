using System;
using Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace EternityEngine
{
	public class Panel : UpdateWhileEnabled
	{
		public RectTransform rectTrs;
		public RectTransform tabRectTrs;
		public RectTransform contentsParentRectTrs;
		public RectTransform contentsRectTrs;
		public RectTransform tabOptionsRectTrs;
		public RectTransform borderRectTrs;
		// public Type type;
		public TabOptionsUpdater tabOptionsUpdater;
		[HideInInspector]
		public bool mouseIsInTab;
		public RectTransform canvasRectTrs;
		public Rect initScreenNormalizedRect;
		public float screenNormalizedBorderRadius;
		[HideInInspector]
		public float borderRadius;
		public static Panel[] instances = new Panel[0];
		static Panel panelOfContentsParentMouseIsIn;
		static Panel resizing;
		Vector2 offDrag;
		bool isDragging;
		DragUpdater dragUpdater;
		RectTransform contentsParentRectTrsCopy;
		ResizeUpdater resizeUpdater;
		const float MIN_SCREEN_NORMALIZED_SIZE = .05f;

		public virtual void Awake ()
		{
			instances = instances.Add(this);
			borderRadius = Mathf.Min(Screen.width * screenNormalizedBorderRadius, Screen.height * screenNormalizedBorderRadius);
			borderRectTrs.sizeDelta = Vector2.one * borderRadius * 2;
			contentsRectTrs.sizeDelta = -Vector2.one * borderRadius * 2;
			rectTrs.sizeDelta = canvasRectTrs.sizeDelta * initScreenNormalizedRect.size;
			rectTrs.sizeDelta = new Vector2((int) rectTrs.sizeDelta.x, (int) rectTrs.sizeDelta.y);
			rectTrs.position = canvasRectTrs.sizeDelta * initScreenNormalizedRect.center;
			rectTrs.position = new Vector2((int) rectTrs.position.x, (int) rectTrs.position.y);
			contentsParentRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsParentRectTrs.localPosition * 2;
		}
		
		public virtual void OnDestroy ()
		{
			instances = instances.Remove(this);
		}

		public override void DoUpdate ()
		{
			Vector2 mousePos = Mouse.current.position.ReadValue();
			Rect borderWorldRect = borderRectTrs.GetWorldRect();
			bool mouseButtonPressed = Mouse.current.leftButton.isPressed;
			bool isResizing = resizing == this;
			if (!borderWorldRect.Contains(mousePos))
			{
				if (!mouseButtonPressed && !isResizing && resizeUpdater != null)
				{
					GameManager.updatables = GameManager.updatables.Remove(resizeUpdater);
					resizeUpdater = null;
				}
			}
			else if (resizeUpdater == null)
			{
				resizeUpdater = new ResizeUpdater(this);
				GameManager.updatables = GameManager.updatables.Add(resizeUpdater);
			}
		}

		public void BeginDrag ()
		{
			isDragging = true;
			offDrag = (Vector2) tabRectTrs.position - Mouse.current.position.ReadValue();
			if (tabOptionsUpdater != null)
				GameManager.updatables = GameManager.updatables.Remove(tabOptionsUpdater);
			contentsParentRectTrsCopy = Instantiate(contentsParentRectTrs, contentsParentRectTrs.parent);
			Destroy(contentsParentRectTrsCopy.GetComponent<EventTrigger>());
			contentsParentRectTrsCopy.GetComponent<Canvas>().overrideSorting = true;
			dragUpdater = new DragUpdater(this);
			GameManager.updatables = GameManager.updatables.Add(dragUpdater);
		}

		public void Drag ()
		{
			if (!isDragging)
				return;
			tabRectTrs.position = offDrag + Mouse.current.position.ReadValue();
			if (panelOfContentsParentMouseIsIn == null || panelOfContentsParentMouseIsIn == this)
			{
				contentsParentRectTrsCopy.position = contentsParentRectTrs.position;
				contentsParentRectTrsCopy.sizeDelta = contentsParentRectTrs.sizeDelta;
			}
			else if (panelOfContentsParentMouseIsIn != this)
			{
				RectTransform contentsParentRectTrsMouseIsIn = panelOfContentsParentMouseIsIn.contentsParentRectTrs;
				Rect contentsParentWorldRectMouseIsIn = contentsParentRectTrsMouseIsIn.GetWorldRect();
				RectTransform previewParentRectTrs = (RectTransform) contentsParentRectTrsCopy.parent;
				Action joinLeft = () => { 
					Rect leftHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.xMin, contentsParentWorldRectMouseIsIn.yMin, contentsParentWorldRectMouseIsIn.center.x, contentsParentWorldRectMouseIsIn.yMax);
					Vector3 localMin = previewParentRectTrs.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMin, leftHalfWorldRect.yMin));
					Vector3 localMax = previewParentRectTrs.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMax, leftHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					contentsParentRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					contentsParentRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					contentsParentRectTrsCopy.localPosition = localMid; };
				Action joinRight = () => { 
					Rect rightHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.center.x, contentsParentWorldRectMouseIsIn.yMin, contentsParentWorldRectMouseIsIn.xMax, contentsParentWorldRectMouseIsIn.yMax);
					Vector3 localMin = previewParentRectTrs.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMin, rightHalfWorldRect.yMin));
					Vector3 localMax = previewParentRectTrs.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMax, rightHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					contentsParentRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					contentsParentRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					contentsParentRectTrsCopy.localPosition = localMid; };
				Action joinBott = () => { 
					Rect bottHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.xMin, contentsParentWorldRectMouseIsIn.yMin, contentsParentWorldRectMouseIsIn.xMax, contentsParentWorldRectMouseIsIn.center.y);
					Vector3 localMin = previewParentRectTrs.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMin, bottHalfWorldRect.yMin));
					Vector3 localMax = previewParentRectTrs.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMax, bottHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					contentsParentRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					contentsParentRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					contentsParentRectTrsCopy.localPosition = localMid; };
				Action joinTop = () => { 
					Rect topHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.xMin, contentsParentWorldRectMouseIsIn.center.y, contentsParentWorldRectMouseIsIn.xMax, contentsParentWorldRectMouseIsIn.yMax);
					Vector3 localMin = previewParentRectTrs.InverseTransformPoint(new Vector3(topHalfWorldRect.xMin, topHalfWorldRect.yMin));
					Vector3 localMax = previewParentRectTrs.InverseTransformPoint(new Vector3(topHalfWorldRect.xMax, topHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					contentsParentRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					contentsParentRectTrsCopy.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					contentsParentRectTrsCopy.localPosition = localMid; };
				Action swap = () => { contentsParentRectTrsCopy.position = contentsParentRectTrsMouseIsIn.position;
					contentsParentRectTrsCopy.sizeDelta = contentsParentRectTrsMouseIsIn.sizeDelta; };
				JoinOrSwapPanels (panelOfContentsParentMouseIsIn, joinLeft, joinRight, joinBott, joinTop, swap);
			}
		}

		public void EndDrag ()
		{
			isDragging = false;
			GameManager.updatables = GameManager.updatables.Remove(dragUpdater);
			if (panelOfContentsParentMouseIsIn != null && panelOfContentsParentMouseIsIn != this)
			{
				RectTransform contentsParentRectTrsMouseIsIn = panelOfContentsParentMouseIsIn.contentsParentRectTrs;
				Rect contentsParentWorldRectMouseIsIn = contentsParentRectTrsMouseIsIn.GetWorldRect();
				Transform parentRectTrs = rectTrs.parent;
				RectTransform panelRectTrsMouseIsIn = panelOfContentsParentMouseIsIn.rectTrs;
				Transform parentOfPanelMouseIsIn = panelRectTrsMouseIsIn.parent;
				Action joinLeft = () => {
					Rect leftHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.xMin, contentsParentWorldRectMouseIsIn.yMin, contentsParentWorldRectMouseIsIn.center.x, contentsParentWorldRectMouseIsIn.yMax);
					Vector3 localMin = parentRectTrs.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMin, leftHalfWorldRect.yMin));
					Vector3 localMax = parentRectTrs.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMax, leftHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y - contentsParentRectTrs.localPosition.y * 2);
					rectTrs.localPosition = localMid - (Vector2) contentsParentRectTrs.localPosition;
					contentsParentRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsParentRectTrs.localPosition * 2;
					
					Rect rightHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.center.x, contentsParentWorldRectMouseIsIn.yMin, contentsParentWorldRectMouseIsIn.xMax, contentsParentWorldRectMouseIsIn.yMax);
					Vector3 panelContentsParentLocalMin = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMin, rightHalfWorldRect.yMin));
					Vector3 panelContentsParentLocalMax = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMax, rightHalfWorldRect.yMax));
					Vector2 panelContentsParentLocalSize = (Vector2) (panelContentsParentLocalMax - panelContentsParentLocalMin);
					Vector2 panelContentsParentLocalMid = (Vector2) ((panelContentsParentLocalMin + panelContentsParentLocalMax) / 2);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelContentsParentLocalSize.x);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelContentsParentLocalSize.y - contentsParentRectTrsMouseIsIn.localPosition.y * 2);
					panelRectTrsMouseIsIn.localPosition = panelContentsParentLocalMid - (Vector2) contentsParentRectTrsMouseIsIn.localPosition;
					contentsParentRectTrsMouseIsIn.sizeDelta = panelRectTrsMouseIsIn.sizeDelta + (Vector2) contentsParentRectTrsMouseIsIn.localPosition * 2;
				};
				Action joinRight = () => {
					Rect rightHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.center.x, contentsParentWorldRectMouseIsIn.yMin, contentsParentWorldRectMouseIsIn.xMax, contentsParentWorldRectMouseIsIn.yMax);
					Vector3 localMin = parentRectTrs.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMin, rightHalfWorldRect.yMin));
					Vector3 localMax = parentRectTrs.InverseTransformPoint(new Vector3(rightHalfWorldRect.xMax, rightHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y - contentsParentRectTrs.localPosition.y * 2);
					rectTrs.localPosition = localMid - (Vector2) contentsParentRectTrs.localPosition;
					contentsParentRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsParentRectTrs.localPosition * 2;
					
					Rect leftHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.xMin, contentsParentWorldRectMouseIsIn.yMin, contentsParentWorldRectMouseIsIn.center.x, contentsParentWorldRectMouseIsIn.yMax);
					Vector3 panelContentsParentLocalMin = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMin, leftHalfWorldRect.yMin));
					Vector3 panelContentsParentLocalMax = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(leftHalfWorldRect.xMax, leftHalfWorldRect.yMax));
					Vector2 panelContentsParentLocalSize = (Vector2) (panelContentsParentLocalMax - panelContentsParentLocalMin);
					Vector2 panelContentsParentLocalMid = (Vector2) ((panelContentsParentLocalMin + panelContentsParentLocalMax) / 2);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelContentsParentLocalSize.x);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelContentsParentLocalSize.y - contentsParentRectTrsMouseIsIn.localPosition.y * 2);
					panelRectTrsMouseIsIn.localPosition = panelContentsParentLocalMid - (Vector2) contentsParentRectTrsMouseIsIn.localPosition;
					contentsParentRectTrsMouseIsIn.sizeDelta = panelRectTrsMouseIsIn.sizeDelta + (Vector2) contentsParentRectTrsMouseIsIn.localPosition * 2;
				};
				Action joinBott = () => {
					Rect bottHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.xMin, contentsParentWorldRectMouseIsIn.yMin, contentsParentWorldRectMouseIsIn.xMax, contentsParentWorldRectMouseIsIn.center.y);
					Vector3 localMin = parentRectTrs.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMin, bottHalfWorldRect.yMin));
					Vector3 localMax = parentRectTrs.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMax, bottHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
					rectTrs.localPosition = localMid;
					contentsParentRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsParentRectTrs.localPosition * 2;
					
					Rect topHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.xMin, contentsParentWorldRectMouseIsIn.center.y, contentsParentWorldRectMouseIsIn.xMax, contentsParentWorldRectMouseIsIn.yMax);
					Vector3 panelContentsParentLocalMin = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(topHalfWorldRect.xMin, topHalfWorldRect.yMin));
					Vector3 panelContentsParentLocalMax = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(topHalfWorldRect.xMax, topHalfWorldRect.yMax));
					Vector2 panelContentsParentLocalSize = (Vector2) (panelContentsParentLocalMax - panelContentsParentLocalMin);
					Vector2 panelContentsParentLocalMid = (Vector2) ((panelContentsParentLocalMin + panelContentsParentLocalMax) / 2);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelContentsParentLocalSize.x);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelContentsParentLocalSize.y - contentsParentRectTrsMouseIsIn.localPosition.y * 2);
					panelRectTrsMouseIsIn.localPosition = panelContentsParentLocalMid - (Vector2) contentsParentRectTrsMouseIsIn.localPosition;
					contentsParentRectTrsMouseIsIn.sizeDelta = panelRectTrsMouseIsIn.sizeDelta + (Vector2) contentsParentRectTrsMouseIsIn.localPosition * 2;
				};
				Action joinTop = () => {
					Rect topHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.xMin, contentsParentWorldRectMouseIsIn.center.y, contentsParentWorldRectMouseIsIn.xMax, contentsParentWorldRectMouseIsIn.yMax);
					Vector3 localMin = parentRectTrs.InverseTransformPoint(new Vector3(topHalfWorldRect.xMin, topHalfWorldRect.yMin));
					Vector3 localMax = parentRectTrs.InverseTransformPoint(new Vector3(topHalfWorldRect.xMax, topHalfWorldRect.yMax));
					Vector2 localSize = (Vector2) (localMax - localMin);
					Vector2 localMid = (Vector2) ((localMin + localMax) / 2);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
					rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y - contentsParentRectTrs.localPosition.y * 2);
					rectTrs.localPosition = localMid - (Vector2) contentsParentRectTrs.localPosition;
					contentsParentRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsParentRectTrs.localPosition * 2;
					
					Rect bottHalfWorldRect = Rect.MinMaxRect(contentsParentWorldRectMouseIsIn.xMin, contentsParentWorldRectMouseIsIn.yMin, contentsParentWorldRectMouseIsIn.xMax, contentsParentWorldRectMouseIsIn.center.y);
					Vector3 panelContentsParentLocalMin = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMin, bottHalfWorldRect.yMin));
					Vector3 panelContentsParentLocalMax = parentOfPanelMouseIsIn.InverseTransformPoint(new Vector3(bottHalfWorldRect.xMax, bottHalfWorldRect.yMax));
					Vector2 panelContentsParentLocalSize = (Vector2) (panelContentsParentLocalMax - panelContentsParentLocalMin);
					Vector2 panelContentsParentLocalMid = (Vector2) ((panelContentsParentLocalMin + panelContentsParentLocalMax) / 2);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelContentsParentLocalSize.x);
					panelRectTrsMouseIsIn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelContentsParentLocalSize.y);
					panelRectTrsMouseIsIn.localPosition = panelContentsParentLocalMid;
					contentsParentRectTrsMouseIsIn.sizeDelta = panelRectTrsMouseIsIn.sizeDelta + (Vector2) contentsParentRectTrsMouseIsIn.localPosition * 2;
				};
				Action swap = () => {
					Vector2 prevPos = rectTrs.position;
					Vector2 prevSize = rectTrs.sizeDelta;
					rectTrs.position = panelOfContentsParentMouseIsIn.rectTrs.position;
					rectTrs.sizeDelta = panelOfContentsParentMouseIsIn.rectTrs.sizeDelta;
					panelOfContentsParentMouseIsIn.rectTrs.position = prevPos;
					panelOfContentsParentMouseIsIn.rectTrs.sizeDelta = prevSize;
				};
				JoinOrSwapPanels (panelOfContentsParentMouseIsIn, joinLeft, joinRight, joinBott, joinTop, swap);
				tabRectTrs.anchoredPosition = Vector3.zero;
				for (int i = 0; i < instances.Length; i ++)
				{
					Panel panel = instances[i];
					panel.MakeFit ();
				}
			}
			if (contentsParentRectTrsCopy != null)
				Destroy(contentsParentRectTrsCopy.gameObject);
			if (mouseIsInTab && tabOptionsUpdater == null)
				OnMouseEnterTab ();
		}

		void JoinOrSwapPanels (Panel panel, Action joinLeft, Action joinRight, Action joinBott, Action joinTop, Action swap)
		{
			Vector2 mousePos = Mouse.current.position.ReadValue();
			RectTransform panelContentsParentRectTrs = panel.contentsParentRectTrs;
			Vector2 normalizedMousePosInContentsParent = new Vector2(Mathf.InverseLerp(panelContentsParentRectTrs.position.x - panelContentsParentRectTrs.sizeDelta.x / 2, panelContentsParentRectTrs.position.x + panelContentsParentRectTrs.sizeDelta.x / 2, mousePos.x),
				Mathf.InverseLerp(panelContentsParentRectTrs.position.y - panelContentsParentRectTrs.sizeDelta.y / 2, panelContentsParentRectTrs.position.y + panelContentsParentRectTrs.sizeDelta.y / 2, mousePos.y));
			if (normalizedMousePosInContentsParent.x < 1f / 3)
			{
				if (normalizedMousePosInContentsParent.y < 1f / 3)
				{
					if (normalizedMousePosInContentsParent.x < normalizedMousePosInContentsParent.y)
						joinLeft ();
					else
						joinBott ();
				}
				else if (normalizedMousePosInContentsParent.y > 2f / 3)
				{
					if (normalizedMousePosInContentsParent.x < 1f - normalizedMousePosInContentsParent.y)
						joinLeft ();
					else
						joinTop ();
				}
				else
					joinLeft ();
			}
			else if (normalizedMousePosInContentsParent.x < 2f / 3)
			{
				if (normalizedMousePosInContentsParent.y < 1f / 3)
					joinBott ();
				else if (normalizedMousePosInContentsParent.y < 2f / 3)
					swap ();
				else
					joinTop ();
			}
			else if (normalizedMousePosInContentsParent.y < 1f / 3)
			{
				if (1f - normalizedMousePosInContentsParent.x < normalizedMousePosInContentsParent.y)
					joinBott ();
				else
					joinRight ();
			}
			else if (normalizedMousePosInContentsParent.y < 2f / 3)
			{
				if (normalizedMousePosInContentsParent.x < 1f / 3)
					joinLeft ();
				else if (normalizedMousePosInContentsParent.x < 2f / 3)
					swap ();
				else
					joinRight ();
			}
			else if (normalizedMousePosInContentsParent.x < normalizedMousePosInContentsParent.y)
				joinTop ();
			else
				joinRight ();
		}

		void MakeFit ()
		{
			Rect canvasWorldRect = canvasRectTrs.GetWorldRect();
			float leftHit = canvasWorldRect.xMin;
			float rightHit = canvasWorldRect.xMax;
			float bottHit = canvasWorldRect.yMin;
			float topHit = canvasWorldRect.yMax;
			Rect worldRect = rectTrs.GetWorldRect();
			Vector2 mid = worldRect.center;
			for (int i = 0; i < instances.Length; i ++)
			{
				Panel panel = instances[i];
				if (panel == this)
					continue;
				Rect panelRect = panel.rectTrs.GetWorldRect();
				Vector2 panelMid = panelRect.center;
				bool overlapVertical = panelRect.yMax > worldRect.yMin && panelRect.yMin < worldRect.yMax;
				bool overlapHorizontal = panelRect.xMax > worldRect.xMin && panelRect.xMin < worldRect.xMax;
				if (mid.x > panelMid.x && overlapVertical)
					leftHit = Mathf.Max(leftHit, panelRect.xMax);
				if (mid.x < panelMid.x && overlapVertical)
					rightHit = Mathf.Min(rightHit, panelRect.xMin);
				if (mid.y > panelMid.y && overlapHorizontal)
					bottHit = Mathf.Max(bottHit, panelRect.yMax);
				if (mid.y < panelMid.y && overlapHorizontal)
					topHit = Mathf.Min(topHit, panelRect.yMin);
			}
			Rect newWorldRect = Rect.MinMaxRect(leftHit, bottHit, rightHit, topHit);
			Transform parentRectTrs = rectTrs.parent;
			Vector2 localMin = parentRectTrs.InverseTransformPoint(newWorldRect.min);
			Vector2 localMax = parentRectTrs.InverseTransformPoint(newWorldRect.max);
			Vector2 localSize = localMax - localMin;
			Vector2 localMid = (localMin + localMax) / 2;
			rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSize.x);
			rectTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSize.y);
			rectTrs.localPosition = localMid;
			contentsParentRectTrs.sizeDelta = rectTrs.sizeDelta + (Vector2) contentsParentRectTrs.localPosition * 2;
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

		public void OnMouseEnterContentsParent ()
		{
			panelOfContentsParentMouseIsIn = this;
		}

		public void OnMouseExitContentsParent ()
		{
			Vector2 worldMousePos = Mouse.current.position.ReadValue();
			for (int i = 0; i < instances.Length; i ++)
			{
				Panel panel = instances[i];
				Rect contentsParentWorldRect = panel.contentsParentRectTrs.GetWorldRect();
				if (contentsParentWorldRect.Contains(worldMousePos))
				{
					panelOfContentsParentMouseIsIn = panel;
					return;
				}
			}
			panelOfContentsParentMouseIsIn = null;
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

		public class ResizeUpdater : IUpdatable
		{
			public Panel panel;
			Rect contentsParentWorldRect;
			bool isResizing;

			public ResizeUpdater (Panel panel)
			{
				this.panel = panel;
				contentsParentWorldRect = panel.contentsParentRectTrs.GetWorldRect();
			}

			public void DoUpdate ()
			{
				Vector2 mousePos = Mouse.current.position.ReadValue();
				if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					Rect innerWorldRect = contentsParentWorldRect.Grow(-Vector2.one * panel.borderRadius * 2);
					isResizing = !innerWorldRect.Contains(mousePos);
				}
				else if (Mouse.current.leftButton.wasReleasedThisFrame)
				{
					// Finish resizing when button is released
					if (isResizing)
					{
						isResizing = false;
						if (resizing == panel)
						{
							panel.MakeFit();
							resizing = null;
						}
					}
				}
				if (isResizing)
				{
					resizing = panel;
					contentsParentWorldRect = panel.contentsParentRectTrs.GetWorldRect();
					Vector2 offNormalizedMousedPos = Rect.PointToNormalized(contentsParentWorldRect, mousePos) - Vector2.one / 2;
					Rect worldRect = panel.rectTrs.GetWorldRect();
					Rect newRect;
					if (Mathf.Abs(offNormalizedMousedPos.x) > Mathf.Abs(offNormalizedMousedPos.y))
					{
						if (offNormalizedMousedPos.x > 0)
							newRect = Rect.MinMaxRect(worldRect.xMin, worldRect.yMin, mousePos.x, worldRect.yMax);
						else
							newRect = Rect.MinMaxRect(mousePos.x, worldRect.yMin, worldRect.xMax, worldRect.yMax);
					}
					else
					{
						if (offNormalizedMousedPos.y > 0)
							newRect = Rect.MinMaxRect(worldRect.xMin, worldRect.yMin, worldRect.xMax, mousePos.y);
						else
							newRect = Rect.MinMaxRect(worldRect.xMin, mousePos.y, worldRect.xMax, worldRect.yMax);
					}
					if (newRect.size.x * newRect.size.y < panel.rectTrs.sizeDelta.x * panel.rectTrs.sizeDelta.y)
					{
						panel.rectTrs.sizeDelta = newRect.size;
						panel.rectTrs.position = newRect.center;
						panel.contentsParentRectTrs.sizeDelta = newRect.size + (Vector2) panel.contentsParentRectTrs.localPosition * 2;
						panel.tabRectTrs.anchoredPosition = Vector3.zero;
						for (int i = 0; i < instances.Length; i ++)
						{
							Panel _panel = instances[i];
							if (_panel != panel)
								_panel.MakeFit ();
						}
					}
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

		// public enum Type
		// {
		// 	Scene,
		// 	Hierarchy,
		// 	Project,
		// 	Inspector 
		// }
	}
}