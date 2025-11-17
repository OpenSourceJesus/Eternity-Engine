using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace EternityEngine
{
	public class ScenePanel : Panel
	{
		public RectTransform viewportRectTrs;
		public RectTransform obsParentRectTrs;
		[HideInInspector]
		public SceneEntry[] entries = new SceneEntry[0];
		[HideInInspector]
		public SceneEntry[] selected = new SceneEntry[0];
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
			Vector2 prevMousePos;

			public Updater (ScenePanel scenePanel)
			{
				this.scenePanel = scenePanel;
			}

			public void DoUpdate ()
			{
				Vector2 mousePos = Mouse.current.position.ReadValue();
				if (Mouse.current.middleButton.isPressed)
					scenePanel.obsParentRectTrs.position += (Vector3) (mousePos - prevMousePos);
				prevMousePos = mousePos;
				if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					int prevSelectedCnt = HierarchyPanel.instances[0].selected.Length;
					for (int i = 0; i < scenePanel.entries.Length; i ++)
					{
						SceneEntry entry = scenePanel.entries[i];
						if (entry.img != null && entry.img.rectTransform.GetWorldRect2D().Contains_Polygon(mousePos))
						{
							if (!Keyboard.current.leftShiftKey.isPressed)
								for (int i2 = 0; i2 < scenePanel.selected.Length; i2 ++)
								{
									SceneEntry _entry = scenePanel.selected[i2];
									if (_entry != entry)
									{
										_entry.SetSelected (false);
										i2 --;
									}
								}
							if (Keyboard.current.leftCtrlKey.isPressed)
								entry.SetSelected (!entry.selected);
							else
								entry.SetSelected (true);
							return;
						}
					}
					for (int i = 0; i < scenePanel.selected.Length; i ++)
					{
						SceneEntry entry = scenePanel.selected[i];
						entry.SetSelected (false);
						i --;
					}
					InspectorPanel.RegenEntries (prevSelectedCnt > 1);
				}
			}
		}
	}
}