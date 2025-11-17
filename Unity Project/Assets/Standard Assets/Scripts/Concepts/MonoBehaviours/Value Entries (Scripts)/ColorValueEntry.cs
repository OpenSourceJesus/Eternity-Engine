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
		public Image valIndicator;
		public GameObject subValueEntriesParentGo;

		void Start ()
		{
			rValueEntry.value.onChanged += () => { valIndicator.color = value.val; };
			gValueEntry.value.onChanged += () => { valIndicator.color = value.val; };
			bValueEntry.value.onChanged += () => { valIndicator.color = value.val; };
			aValueEntry.value.onChanged += () => { valIndicator.color = value.val; };
		}

		void OnDestroy ()
		{
			rValueEntry.value.onChanged -= () => { valIndicator.color = value.val; };
			gValueEntry.value.onChanged -= () => { valIndicator.color = value.val; };
			bValueEntry.value.onChanged -= () => { valIndicator.color = value.val; };
			aValueEntry.value.onChanged -= () => { valIndicator.color = value.val; };
		}

		public void ToggleSubValueEntries ()
		{
			subValueEntriesParentGo.SetActive(!subValueEntriesParentGo.activeSelf);
		}
	}
}