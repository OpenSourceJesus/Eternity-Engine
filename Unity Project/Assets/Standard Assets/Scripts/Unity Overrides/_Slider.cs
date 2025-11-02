using TMPro;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

public class _Slider : _Selectable
{
	public Slider slider;
	public TMP_Text displayValueText;
	public RectTransform slidingAreaRectTrs;
	public float[] snapValues = new float[0];
	[HideInInspector]
	public int indexOfCurrentSnapValue;
	string initDisplayValueTextString;
	
	void Awake ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		if (displayValueText != null)
		{
			initDisplayValueTextString = displayValueText.text;
			SetDisplayValue ();
		}
		OnValueChanged ();
		if (snapValues.Length > 0)
			indexOfCurrentSnapValue = MathfExtensions.GetIndexOfClosestNumber(slider.value, snapValues);
	}

	public void OnValueChanged ()
	{
		if (snapValues.Length > 0)
			slider.value = MathfExtensions.GetClosestNumber(slider.value, snapValues);
	}
	
	public void SetDisplayValue ()
	{
		if (displayValueText != null)
			displayValueText.text = initDisplayValueTextString + slider.value;
	}
}
