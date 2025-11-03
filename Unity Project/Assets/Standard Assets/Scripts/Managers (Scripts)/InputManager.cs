using UnityEngine.InputSystem;

namespace Frogger
{
	public class InputManager : SingletonMonoBehaviour<InputManager>
	{
		public InputDevice inputDevice;
		public InputSettings settings;
		public InputActionAsset inputActionAsset;

		public override void Awake ()
		{
			base.Awake ();
			// uiMovement = inputActionAsset.FindAction("UI Movement");
			// uiMovement.Enable();
		}
	}
}