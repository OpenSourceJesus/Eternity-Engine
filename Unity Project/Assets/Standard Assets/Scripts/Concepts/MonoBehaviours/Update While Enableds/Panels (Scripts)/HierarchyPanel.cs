using Extensions;
using UnityEngine;

namespace EternityEngine
{
	public class HierarchyPanel : Panel
	{
		public new static HierarchyPanel[] instances = new HierarchyPanel[0];
		public Transform entriesParent;

		public override void Awake ()
		{
			base.Awake ();
			instances = instances.Add(this);
		}
		
		public override void OnDestroy ()
		{
			base.OnDestroy ();
			instances = instances.Remove(this);
		}
	}
}