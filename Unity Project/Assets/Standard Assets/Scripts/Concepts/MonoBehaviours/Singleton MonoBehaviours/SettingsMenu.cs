using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class SettingsMenu : SingletonMonoBehaviour<SettingsMenu>
	{
		public Slider volumeSlider;
		public Button muteButton;
		public Slider timeSpeedSlider;
		public static float Volume
		{
			get
			{
				return SaveAndLoadManager.GetFloat("Volume", 1);
			}
			set
			{
				AudioListener.volume = value;
				SaveAndLoadManager.SetFloat ("Volume", value);
				// SaveAndLoadManager.Save ();
			}
		}
		public static bool EnableSound
		{
			get
			{
				return SaveAndLoadManager.GetBool("Enable sound", true);
			}
			set
			{
				AudioListener.pause = !value;
				SaveAndLoadManager.SetBool ("Enable sound", value);
				// SaveAndLoadManager.Save ();
			}
		}
		public static float TimeSpeed
		{
			get
			{
				return SaveAndLoadManager.GetFloat("Time speed", GameManager.instance.timeSpeed);
			}
			set
			{
				GameManager.instance.timeSpeed = value;
				SaveAndLoadManager.SetFloat ("Time speed", value);
				// SaveAndLoadManager.Save ();
			}
		}

		public override void Awake ()
		{
			base.Awake ();
			gameObject.SetActive(false);
			SaveAndLoadManager.Init ();
			volumeSlider.value = Volume;
			if (!EnableSound)
				muteButton.onClick.Invoke();
			timeSpeedSlider.value = TimeSpeed;
		}
	}
}