using TMPro;
using UnityEngine;

namespace Frogger
{
	public class Sign : MonoBehaviour
	{
		[Multiline(5)]
		public string textStrIfNotUsingGamepad;
		[Multiline(5)]
		public string textStrIfUsingGamepad;
		public GameObject textPanel;
		public TMP_Text textIfNotUsingGamepad;
		public TMP_Text textIfUsingGamepad;

		void OnTriggerEnter2D (Collider2D other)
		{
			textIfUsingGamepad.text = textStrIfUsingGamepad;
			textIfNotUsingGamepad.text = textStrIfNotUsingGamepad;
			textPanel.SetActive(true);
		}

		void OnTriggerExit2D (Collider2D other)
		{
			textPanel.SetActive(false);
		}
	}
}