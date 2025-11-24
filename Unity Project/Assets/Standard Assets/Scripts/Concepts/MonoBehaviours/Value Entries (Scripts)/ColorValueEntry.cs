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
		public GameObject subValueEntriesParentGo;

		void Start ()
		{
			rValueEntry.value.onChanged += () => { valIndctr.color = value.val; };
			gValueEntry.value.onChanged += () => { valIndctr.color = value.val; };
			bValueEntry.value.onChanged += () => { valIndctr.color = value.val; };
			aValueEntry.value.onChanged += () => { valIndctr.color = value.val; };
		}

		void OnDestroy ()
		{
			rValueEntry.value.onChanged -= () => { valIndctr.color = value.val; };
			gValueEntry.value.onChanged -= () => { valIndctr.color = value.val; };
			bValueEntry.value.onChanged -= () => { valIndctr.color = value.val; };
			aValueEntry.value.onChanged -= () => { valIndctr.color = value.val; };
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