using UnityEngine.UI;

namespace EternityEngine
{
	public class RigidBodyEntry : InspectorEntry
	{
		public Button[] typesButtons = new Button[0];

		public override void UpdateDisplay (_Component component)
		{
			base.UpdateDisplay (component);
			RigidBody rigidBody = (RigidBody) component;
			for (int i = 0; i < typesButtons.Length; i ++)
			{
				Button typeButton = typesButtons[i];
				typeButton.interactable = rigidBody.type.val != i;
			}
		}
	}
}