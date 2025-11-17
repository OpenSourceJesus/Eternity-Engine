using UnityEngine;
using UnityEngine.EventSystems;

class ColorComponentValueIndicator : UILineRenderer
{
	protected override void OnRectTransformDimensionsChange ()
	{
		float halfWidth = rectTransform.rect.width / 2;
		points = new Vector2[] { Vector3.left * halfWidth, Vector3.right * halfWidth };
	}
}