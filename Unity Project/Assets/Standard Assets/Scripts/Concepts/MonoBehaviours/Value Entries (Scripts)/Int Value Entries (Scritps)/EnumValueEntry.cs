using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class EnumValueEntry : IntValueEntry
	{
		public TMP_Text txt;
		public GameObject optionsGo;
		public Button[] buttons = new Button[0];
		public TMP_Text[] buttonsTxts = new TMP_Text[0];

		public void ToggleOptions ()
		{
			optionsGo.SetActive(!optionsGo.activeSelf);
		}

		public override void TrySet (string txt)
		{
			int newVal = int.Parse(txt);
			UpdateDisplay (newVal);
			Value<int>[] targets = TargetValues;
			if (targets.Length == 0)
				return;
			for (int i = 0; i < targets.Length; i ++)
			{
				Value<int> target = targets[i];
				if (target == null || target.val == newVal)
					continue;
				target.val = newVal;
				target._OnChanged ();
			}
		}

		public override void UpdateDisplay (int val)
		{
			for (int i = 0; i < buttons.Length; i ++)
			{
				Button button = buttons[i];
				button.interactable = val != i;
			}
			txt.text = buttonsTxts[val].text;
		}
	}
}