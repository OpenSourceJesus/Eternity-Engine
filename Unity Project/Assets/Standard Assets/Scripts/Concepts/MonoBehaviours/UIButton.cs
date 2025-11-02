using Extensions;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Frogger
{
	public class UIButton : Button, IUpdatable
	{
		public Image image;
		public GameObject goWhenPressed;
		public GameObject goWhenNotPressed;

		public virtual void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public virtual void DoUpdate ()
		{
			if (!interactable || Gamepad.current != null)
			{
				if (!interactable)
					image.color = colors.disabledColor;
				return;
			}
			if (IsPressed())
				StartPress ();
			else
				EndPress ();
		}

		public void StartPress ()
		{
			goWhenPressed.SetActive(true);
			if (goWhenNotPressed != null)
				goWhenNotPressed.SetActive(false);
			image.color = image.color.SetAlpha(0);
		}

		public void EndPress ()
		{
			if (goWhenPressed != null)
				goWhenPressed.SetActive(false);
			if (goWhenNotPressed != null)
				goWhenNotPressed.SetActive(true);
			image.color = image.color.SetAlpha(1);
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}