using UnityEngine;

namespace EternityEngine
{
	[ExecuteInEditMode]
	public class Spawnable : MonoBehaviour, ISpawnable
	{
		public Transform trs;
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}
	}
}