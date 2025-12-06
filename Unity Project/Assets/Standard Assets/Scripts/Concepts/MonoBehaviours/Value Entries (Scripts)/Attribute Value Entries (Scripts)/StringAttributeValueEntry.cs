using System.Collections.Generic;

namespace EternityEngine
{
	public class StringAttributeValueEntry : AttributeValueEntry<string>
	{
		void Start ()
		{
			for (int i = 0; i < elts.Length; i ++)
			{
				StringAttributeElementValueEntry elt = (StringAttributeElementValueEntry) elts[i];
				elt.onMouseDown += OnElementMouseDown;
				elt.onMouseUp += OnElementMouseUp;
				elt.inputField.onValueChanged.AddListener((string s) => { OnElementValueChanged (elt); });
				elt.valueEntry.inputField.onValueChanged.AddListener((string s) => { OnElementValueChanged (elt); });
			}
		}

		public override ValueEntry<KeyValuePair<string, string>> AddElement ()
		{
			AttributeElementValueEntry<string> elt = (AttributeElementValueEntry<string>) base.AddElement();
			elt.valueEntry.inputField.onValueChanged.AddListener((string s) => { OnElementValueChanged (elt); });
			return elt;
		}
	}
}