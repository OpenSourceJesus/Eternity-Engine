using Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EternityEngine
{
	public class ScenePanel : Panel
	{
		public RectTransform viewportRectTrs;
		public RectTransform obsParentRectTrs;
		public new static ScenePanel[] instances = new ScenePanel[0];
		Updater updater;

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

		public void OnMouseEnterViewport ()
		{
			updater = new Updater(this);
			GameManager.updatables = GameManager.updatables.Add(updater);
		}

		public void OnMouseExitViewport ()
		{
			GameManager.updatables = GameManager.updatables.Remove(updater);
		}

		class Updater : IUpdatable
		{
			ScenePanel scenePanel;

			public Updater (ScenePanel ScenePanel)
			{
				this.scenePanel = scenePanel;
			}

			public void DoUpdate ()
			{
				
			}
		}
	}
}