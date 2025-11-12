using UnityEngine;

namespace EternityEngine
{
	public class _Component : MonoBehaviour
	{
		public int typeIdx;
		public Transform trs;
		[HideInInspector]
        public _Object ob;
        public InspectorEntry inspectorEntryPrefab;
		[HideInInspector]
		public bool collapsed;
		public FloatValue[] floatValues = new FloatValue[0];
		public Vector3Value[] vector3Values = new Vector3Value[0];
		[HideInInspector]
        public InspectorEntry[] inspectorEntries = new InspectorEntry[0];
	}
}