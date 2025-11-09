using Extensions;
using UnityEngine;

namespace EternityEngine
{
	public class HierarchyPanel : Panel
	{
		public Transform entriesParent;
		public HierarchyEntry[] entries = new HierarchyEntry[0];
		public HierarchyEntry[] selected = new HierarchyEntry[0];
		public new static HierarchyPanel[] instances = new HierarchyPanel[0];

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