using Extensions;
using UnityEngine.InputSystem;

namespace EternityEngine
{
	public class ActivateObjectBasedOnUsingGamepad : UpdateWhileEnabled
	{
		public bool disableIfUsing;

		void Awake ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}
		
		public override void DoUpdate ()
		{
			gameObject.SetActive(Gamepad.current != null != disableIfUsing);
		}

		public override void OnEnable ()
		{
		}

		public override void OnDisable ()
		{
		}
	}
}