namespace EternityEngine
{
	public class IntArrayValueEntry : ArrayValueEntry<int>
	{
		public override void Start ()
		{
			base.Start ();
			for (int i = 0; i < elts.Length; i ++)
			{
				ValueEntry<int> elt = elts[i];
				elt.onMouseDown += OnElementMouseDown;
				elt.onMouseUp += OnElementMouseUp;
				elt.inputField.onValueChanged.AddListener((string s) => { OnElementValueChanged (elt); });
			}
		}
	}
}