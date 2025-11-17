using TMPro;
using UnityEngine;

namespace EternityEngine
{
	public class SetableValue : MonoBehaviour
	{
		public TMP_InputField valueSetter;
		
		public virtual void TrySet (string text)
		{
			if (valueSetter != null)
				valueSetter.text = text;
		}
	}
}