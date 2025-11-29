using UnityEngine;

namespace EternityEngine
{
	public class Vector2ArrayValueEntry : ArrayValueEntry<Vector2>
	{
		public override void Start ()
		{
			base.Start ();
			for (int i = 0; i < elts.Length; i ++)
			{
				Vector2ValueEntry elt = (Vector2ValueEntry) elts[i];
				elt.onMouseDown += OnElementMouseDown;
				elt.onMouseUp += OnElementMouseUp;
				elt.xValueEntry.inputField.onValueChanged.AddListener((string s) => { OnElementValueChanged (elt); });
				elt.yValueEntry.inputField.onValueChanged.AddListener((string s) => { OnElementValueChanged (elt); });
			}
		}
	}
}