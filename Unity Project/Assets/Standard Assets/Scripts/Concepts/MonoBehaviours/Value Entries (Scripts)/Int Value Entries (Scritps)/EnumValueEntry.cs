using TMPro;
using System;
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
		public GameObjectGroup[] goGroups = new GameObjectGroup[0];

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
				if (val == i && goGroups.Length > i)
				{
					for (int i2 = 0; i2 < goGroups.Length; i2 ++)
					{
						GameObjectGroup goGroup = goGroups[i2];
						goGroup.SetActive (false);
					}
					goGroups[i].SetActive (true);
				}
			}
			txt.text = buttonsTxts[val].text;
		}

		[Serializable]
		public class GameObjectGroup
		{
			public GameObject[] gos = new GameObject[0];

			public void SetActive (bool active)
			{
				for (int i = 0; i < gos.Length; i ++)
				{
					GameObject go = gos[i];
					go.SetActive(active);
				}
			}
		}
	}
}