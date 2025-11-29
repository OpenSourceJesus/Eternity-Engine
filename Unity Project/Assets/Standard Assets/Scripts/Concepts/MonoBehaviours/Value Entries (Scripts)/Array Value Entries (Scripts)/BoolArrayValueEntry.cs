namespace EternityEngine
{
	public class BoolArrayValueEntry : ArrayValueEntry<bool>
	{
		public override void Start ()
		{
			base.Start ();
			for (int i = 0; i < elts.Length; i ++)
			{
				BoolValueEntry elt = (BoolValueEntry) elts[i];
				elt.onMouseDown += OnElementMouseDown;
				elt.onMouseUp += OnElementMouseUp;
				elt.toggle.onValueChanged.AddListener((bool b) => { OnElementValueChanged (elt); });
			}
		}
	}
}