using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class ColorValueEntry : ValueEntry<Color>
	{
		public ColorComponentValueEntry rValueEntry;
		public ColorComponentValueEntry gValueEntry;
		public ColorComponentValueEntry bValueEntry;
		public ColorComponentValueEntry aValueEntry;
		public Image valIndctr;
		public GameObject multipleValuesIndctrGo;
		public GameObject subValueEntriesParentGo;

		void Start ()
		{
			rValueEntry.value.onChanged += OnChanged;
			gValueEntry.value.onChanged += OnChanged;
			bValueEntry.value.onChanged += OnChanged;
			aValueEntry.value.onChanged += OnChanged;
		}

		void OnDestroy ()
		{
			if (rValueEntry.value == null)
				return;
			rValueEntry.value.onChanged -= OnChanged;
			gValueEntry.value.onChanged -= OnChanged;
			bValueEntry.value.onChanged -= OnChanged;
			aValueEntry.value.onChanged -= OnChanged;
		}

		void OnChanged ()
		{
			if (value != null)
			{
				valIndctr.color = value.val;
				if (rValueEntry.inputField.text != "—" && gValueEntry.inputField.text != "—" && bValueEntry.inputField.text != "—" && aValueEntry.inputField.text != "—")
					multipleValuesIndctrGo.SetActive(false);
			}
		}

		public override void UpdateDisplay (Color val)
		{
			valIndctr.color = val;
		}

		public void ToggleSubValueEntries ()
		{
			subValueEntriesParentGo.SetActive(!subValueEntriesParentGo.activeSelf);
		}
	}
}