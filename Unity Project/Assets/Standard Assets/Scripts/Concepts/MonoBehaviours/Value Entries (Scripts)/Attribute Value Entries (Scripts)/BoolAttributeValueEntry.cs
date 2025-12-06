using System.Collections.Generic;

namespace EternityEngine
{
	public class BoolAttributeValueEntry : AttributeValueEntry<bool>
	{
		void Start ()
		{
			for (int i = 0; i < elts.Length; i ++)
			{
				BoolAttributeElementValueEntry elt = (BoolAttributeElementValueEntry) elts[i];
				elt.onMouseDown += OnElementMouseDown;
				elt.onMouseUp += OnElementMouseUp;
				elt.inputField.onValueChanged.AddListener((string s) => { OnElementValueChanged (elt); });
				((BoolValueEntry) elt.valueEntry).toggle.onValueChanged.AddListener((bool b) => { OnElementValueChanged (elt); });
			}
		}

		public override ValueEntry<KeyValuePair<string, bool>> AddElement ()
		{
			AttributeElementValueEntry<bool> elt = (AttributeElementValueEntry<bool>) base.AddElement();
			((BoolValueEntry) elt.valueEntry).toggle.onValueChanged.AddListener((bool b) => { OnElementValueChanged (elt); });
			return elt;
		}
	}
}