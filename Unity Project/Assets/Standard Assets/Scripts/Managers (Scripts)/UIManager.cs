using TMPro;
using Frogger;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIManager : SingletonUpdateWhileEnabled<UIManager>
{
	public _Selectable currentSelected;
	public ComplexTimer multiplyBrightness;
	public float angleEffectiveness;
	public float distanceEffectiveness;
	public Timer repeatTimer;
	public bool useAngleAndDistanceEffectiveness;
	public EventSystem eventSystem;
	Vector2 inputVector;
	Vector2 previousInputVector;
	bool inControlMode;
	bool controllingWithJoystick;
	bool leftClickInput;
	bool previousLeftClickInput;
	TMP_InputField currentInputField;
	bool previousSubmitInput;
	bool isSubmitting;
	float initRepeatInterval;
	float scrollbarValueInterval = 0.01f;

	public override void Awake ()
	{
		base.Awake ();
		initRepeatInterval = repeatTimer.duration;
		repeatTimer.onFinished += () => { HandleChangeSelected (true); ControlSelected (); };
	}

	void OnDestroy ()
	{
		repeatTimer.onFinished -= () => { HandleChangeSelected (true); ControlSelected (); };
	}

	public override void DoUpdate ()
	{
		if (Gamepad.current == null)
		{
			eventSystem.enabled = true;
			ColorSelected (currentSelected, 1);
			previousLeftClickInput = leftClickInput;
			return;
		}
		// Enable EventSystem only while actively editing an input field
		eventSystem.enabled = currentInputField != null && !currentInputField.readOnly;
		// eventSystem.enabled = false;
		leftClickInput = (Mouse.current != null && Mouse.current.leftButton.isPressed) || (Touchscreen.current != null && Touchscreen.current.touches.Count > 0);
		if (currentSelected != null)
		{
			if (!CanSelectSelectable(currentSelected))
			{
				ColorSelected (currentSelected, 1);
				HandleChangeSelected (false);
			}
			ColorSelected (currentSelected, multiplyBrightness.GetValue());
			HandleMouseInput ();
			HandleMovementInput ();
			HandleSubmitSelected ();
		}
		else
			HandleChangeSelected (false);
		previousLeftClickInput = leftClickInput;
	}

	bool CanSelectSelectable (_Selectable selectable)
	{
		return _Selectable.instances.Contains(selectable) && selectable.selectable.IsInteractable() && selectable.canvas.enabled;
	}

	public bool IsMousedOverSelectable (_Selectable selectable)
	{
		return IsMousedOverRectTransform(selectable.rectTrs, selectable.canvas, selectable.canvasRectTrs);
	}

	bool IsMousedOverRectTransform (RectTransform rectTrs, Canvas canvas, RectTransform canvasRectTrs)
	{
		if (Mouse.current == null)
			return false;
		Vector2 mousePosition = Mouse.current.position.ReadValue();
		if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
			return rectTrs.GetRectInCanvasNormalized(canvasRectTrs).Contains(canvasRectTrs.GetWorldRect().ToNormalizedPosition(mousePosition));
		else
			return rectTrs.GetRectInCanvasNormalized(canvasRectTrs).Contains(canvasRectTrs.GetWorldRect().ToNormalizedPosition(canvas.worldCamera.ScreenToWorldPoint(mousePosition)));
	}

	void HandleMouseInput ()
	{
		if (Gamepad.current != null)
			return;
		if (!leftClickInput && previousLeftClickInput && !controllingWithJoystick)
			inControlMode = false;
		foreach (_Selectable selectable in _Selectable.instances)
		{
			if (currentSelected != selectable && IsMousedOverSelectable(selectable) && CanSelectSelectable(selectable))
			{
				ChangeSelected (selectable);
				return;
			}
		}
		if (leftClickInput)
		{
			if (currentInputField != null)
				currentInputField.readOnly = true;
			_Slider slider = currentSelected.GetComponent<_Slider>();
			if (slider != null)
			{
				Vector2 mousePosition;
				if (Mouse.current.leftButton.isPressed)
					mousePosition = Mouse.current.position.ReadValue();
				else
					mousePosition = Touchscreen.current.primaryTouch.position.ReadValue();
				Vector2 closestPointToMouseCanvasNormalized = new Vector2();
				if (slider.canvas.renderMode == RenderMode.ScreenSpaceOverlay || (slider.canvas.renderMode == RenderMode.ScreenSpaceCamera && slider.canvas.worldCamera == null))
					closestPointToMouseCanvasNormalized = slider.slidingAreaRectTrs.GetRectInCanvasNormalized(slider.canvasRectTrs).ClosestPoint(slider.canvasRectTrs.GetWorldRect().ToNormalizedPosition(mousePosition));
				else
					closestPointToMouseCanvasNormalized = slider.slidingAreaRectTrs.GetRectInCanvasNormalized(slider.canvasRectTrs).ClosestPoint(slider.canvasRectTrs.GetWorldRect().ToNormalizedPosition(slider.canvas.worldCamera.ScreenToWorldPoint(mousePosition)));
				float normalizedValue = slider.slidingAreaRectTrs.GetRectInCanvasNormalized(slider.canvasRectTrs).ToNormalizedPosition(closestPointToMouseCanvasNormalized).x;
				slider.slider.value = Mathf.Lerp(slider.slider.minValue, slider.slider.maxValue, normalizedValue);
				if (slider.snapValues.Length > 0)
					slider.slider.value = MathfExtensions.GetClosestNumber(slider.slider.value, slider.snapValues);
			}
			else
			{
				TMP_InputField inputField = currentSelected.GetComponent<TMP_InputField>();
				if (inputField != null)
				{
					currentInputField = inputField;
					currentInputField.readOnly = false;
					currentInputField.ActivateInputField();
				}
			}
		}
	}

	void HandleMovementInput ()
	{
		inputVector = InputManager.uiMovement.ReadValue<Vector2>();
		if (inputVector.magnitude > InputManager.instance.settings.defaultDeadzoneMin)
		{
			if (previousInputVector.magnitude <= InputManager.instance.settings.defaultDeadzoneMin)
			{
				if (currentInputField != null && !currentInputField.readOnly)
				{
					currentInputField.readOnly = true;
					currentInputField.DeactivateInputField();
				}
				HandleChangeSelected (true);
				ControlSelected ();
				repeatTimer.Reset ();
				repeatTimer.Start ();
			}
		}
		else
			repeatTimer.Stop ();
		previousInputVector = inputVector;
	}

	void HandleChangeSelected (bool useInputVector = true)
	{
		if (currentSelected == null || inControlMode || (currentInputField != null && !currentInputField.readOnly))
			return;
		List<_Selectable> otherSelectables = new List<_Selectable>(currentSelected.canNavigateTo);
		if (useAngleAndDistanceEffectiveness)
			otherSelectables = new List<_Selectable>(_Selectable.instances);
		otherSelectables.Remove(currentSelected);
		if (otherSelectables.Count == 0)
			return;
		_Selectable nextSelected = otherSelectables[0];
		float maxSelectableAttractiveness = GetAttractivenessOfSelectable(nextSelected, useInputVector);
		for (int i = 1; i < otherSelectables.Count; i ++)
		{
			_Selectable selectable = otherSelectables[i];
			float selectableAttractiveness = GetAttractivenessOfSelectable(selectable, useInputVector);
			if (selectableAttractiveness > maxSelectableAttractiveness)
			{
				maxSelectableAttractiveness = selectableAttractiveness;
				nextSelected = selectable;
			}
		}
		ChangeSelected (nextSelected);
	}

	public void ChangeSelected (_Selectable selectable)
	{
		if (inControlMode)
			return;
		ColorSelected (currentSelected, 1);
		UIButton uiButton = currentSelected.GetComponent<UIButton>();
		if (uiButton != null)
			uiButton.EndPress ();
		if (currentSelected.onDeslect != null)
			currentSelected.onDeslect.Invoke();
		currentSelected = selectable;
		currentSelected.selectable.Select();
		multiplyBrightness.JumpToStart ();
		isSubmitting = false;
		_Slider slider = selectable as _Slider;
		_Scrollbar scrollbar = selectable as _Scrollbar;
		if ((slider == null || slider.snapValues.Length > 0) && scrollbar == null)
			repeatTimer.duration = initRepeatInterval;
		ScrollRect scrollRect = selectable.rectTrs.GetComponentInParent<ScrollRect>();
		if (scrollRect != null && selectable != scrollRect.verticalScrollbar.GetComponentInParent<_Selectable>())
		{
			Rect viewportRect = scrollRect.viewport.GetWorldRect();
			Rect rect = viewportRect;
			Vector2 center = rect.center;
			rect.height -= scrollRect.content.GetWorldRect().size.y;
			rect.center = center;
			while (selectable.rectTrs.GetWorldRect().yMin < scrollRect.viewport.GetWorldRect().yMin)
			{
				scrollRect.verticalScrollbar.value -= scrollbarValueInterval;
				Canvas.ForceUpdateCanvases();
			}
			while (selectable.rectTrs.GetWorldRect().yMax > scrollRect.viewport.GetWorldRect().yMax)
			{
				scrollRect.verticalScrollbar.value += scrollbarValueInterval;
				Canvas.ForceUpdateCanvases();
			}
		}
		currentInputField = selectable.GetComponent<TMP_InputField>();
		if (currentInputField != null)
			currentInputField.readOnly = true;
		if (currentSelected.onSelect != null)
			currentSelected.onSelect.Invoke();
	}

	void HandleSubmitSelected ()
	{
		bool submitInput = InputManager.submit.IsPressed();
		if (CanSelectSelectable(currentSelected))
		{
			if ((submitInput && !previousSubmitInput) || (IsMousedOverSelectable(currentSelected) && leftClickInput && !previousLeftClickInput))
			{
				UIButton uiButton = currentSelected.GetComponent<UIButton>();
				if (uiButton != null)
					uiButton.StartPress ();
				isSubmitting = true;
			}
			else if (isSubmitting && ((!submitInput && previousSubmitInput) || (IsMousedOverSelectable(currentSelected) && !leftClickInput && previousLeftClickInput)))
			{
				_Slider slider = currentSelected as _Slider;
				if (slider != null)
				{
					controllingWithJoystick = previousSubmitInput;
					inControlMode = !inControlMode;
				}
				else
				{
					_Scrollbar scrollbar = currentSelected as _Scrollbar;
					if (scrollbar != null)
					{
						controllingWithJoystick = previousSubmitInput;
						inControlMode = !inControlMode;
					}
					else
					{
						Button button = currentSelected.GetComponent<Button>();
						if (button != null)
						{
							UIButton uiButton = button as UIButton;
							if (uiButton != null)
								uiButton.EndPress ();
							button.onClick.Invoke();
						}
						else
						{
							Toggle toggle = currentSelected.GetComponent<Toggle>();
							if (toggle != null)
								toggle.isOn = !toggle.isOn;
							else
							{
								TMP_InputField inputField = currentSelected.GetComponent<TMP_InputField>();
								if (inputField != null)
								{
									if (inputField.readOnly)
									{
										inputField.readOnly = false;
										inputField.ActivateInputField();
									}
									else
									{
										inputField.readOnly = true;
										inputField.DeactivateInputField();
									}
								}
							}
						}
					}
				}
			}
		}
		previousSubmitInput = submitInput;
	}

	void ControlSelected ()
	{
		if (!inControlMode)
			return;
		_Slider slider = currentSelected as _Slider;
		if (slider != null)
		{
			if (slider.snapValues.Length > 0)
			{
				slider.indexOfCurrentSnapValue = Mathf.Clamp(slider.indexOfCurrentSnapValue + MathfExtensions.Sign(inputVector.x), 0, slider.snapValues.Length - 1);
				slider.slider.value = slider.snapValues[slider.indexOfCurrentSnapValue];
			}
			else
			{
				repeatTimer.duration = 0;
				slider.slider.normalizedValue += inputVector.x * Time.unscaledDeltaTime;
			}
		}
		else
		{
			_Scrollbar scrollbar = currentSelected as _Scrollbar;
			if (scrollbar != null)
			{
				repeatTimer.duration = 0;
				scrollbar.scrollbar.value += inputVector.y * Time.unscaledDeltaTime;
			}
		}
	}

	float GetAttractivenessOfSelectable (_Selectable selectable, bool useInputVector = true)
	{
		if (!CanSelectSelectable(selectable))
			return -Mathf.Infinity;
		float attractiveness = selectable.priority;
		if (useInputVector)
		{
			Vector2 vectorToSelectable = GetVectorToSelectable(selectable);
			float angleAttractiveness = 180f - Vector2.Angle(inputVector, vectorToSelectable);
			float distanceAttractiveness = 0;
			if (useAngleAndDistanceEffectiveness)
			{
				angleAttractiveness *= angleEffectiveness;
				distanceAttractiveness = vectorToSelectable.magnitude * distanceEffectiveness;
			}
			attractiveness += angleAttractiveness - distanceAttractiveness;
		}
		return attractiveness;
	}

	Vector2 GetVectorToSelectable (_Selectable selectable)
	{
		return selectable.rectTrs.GetWorldRect().center - currentSelected.rectTrs.GetWorldRect().center;
	}

	void ColorSelected (_Selectable selectable, float multiplyBrightness)
	{
		if (selectable.image.color.a == 0)
			return;
		if (isSubmitting)
			selectable.image.color = selectable.selectable.colors.pressedColor.Multiply(multiplyBrightness);
		else
			selectable.image.color = selectable.selectable.colors.normalColor.Multiply(multiplyBrightness);
	}
}