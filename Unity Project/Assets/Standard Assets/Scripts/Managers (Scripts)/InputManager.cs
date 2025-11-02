using UnityEngine.InputSystem;

namespace Frogger
{
	public class InputManager : SingletonMonoBehaviour<InputManager>
	{
		public InputDevice inputDevice;
		public InputSettings settings;
		public InputActionAsset inputActionAsset;
		public static InputAction aim;
		public static InputAction move;
		public static InputAction shoot;
		public static InputAction interact;
		public static InputAction toggleWorldMap;
		public static InputAction submit;
		public static InputAction uiMovement;

		public override void Awake ()
		{
			base.Awake ();
			move = inputActionAsset.FindAction("Move");
			move.Enable();
			aim = inputActionAsset.FindAction("Aim");
			aim.Enable();
			shoot = inputActionAsset.FindAction("Shoot");
			shoot.Enable();
			interact = inputActionAsset.FindAction("Interact");
			interact.Enable();
			toggleWorldMap = inputActionAsset.FindAction("Toggle World Map");
			toggleWorldMap.Enable();
			submit = inputActionAsset.FindAction("Submit");
			submit.Enable();
			uiMovement = inputActionAsset.FindAction("UI Movement");
			uiMovement.Enable();
		}
	}
}