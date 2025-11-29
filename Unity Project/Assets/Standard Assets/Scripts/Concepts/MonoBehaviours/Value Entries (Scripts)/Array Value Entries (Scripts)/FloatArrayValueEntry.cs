namespace EternityEngine
{
	public class FloatArrayValueEntry : ArrayValueEntry<float>
	{
		public override void Start ()
		{
			base.Start ();
			for (int i = 0; i < elts.Length; i ++)
			{
				ValueEntry<float> elt = elts[i];
				elt.onMouseDown += OnElementMouseDown;
				elt.onMouseUp += OnElementMouseUp;
				elt.inputField.onValueChanged.AddListener((string s) => { OnElementValueChanged (elt); });
			}
		}
	}
}