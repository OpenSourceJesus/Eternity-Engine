using System;
using System.IO;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Image : _Component
	{
		public new Data _Data
		{
			get
			{
				if (data == null)
					data = new Data();
				return (Data) data;
			}
			set
			{
				data = value;
			}
		}
		[HideInInspector]
		public Image[] imgs = new Image[0];
		public StringValue path;
		public Vector2Value pivot;
		public ColorValue tint;
		[HideInInspector]
		public Texture2D tex;

		public override void Init ()
		{
			base.Init ();
			imgs = new Image[ScenePanel.instances.Length];
			imgs[0] = sceneEntry.img;
			tex = new Texture2D(1, 1);
			Transform obTrs = ob.trs;
			Transform sceneEntryTrs = sceneEntry.rectTrs;
			sceneEntryTrs.position = obTrs.position;
			sceneEntryTrs.eulerAngles = obTrs.eulerAngles;
			sceneEntryTrs.localScale = obTrs.localScale;
			for (int i = 1; i < ScenePanel.instances.Length; i ++)
			{
				ScenePanel scenePanel = ScenePanel.instances[i];
				SceneEntry _sceneEntry = Instantiate(sceneEntry, scenePanel.obsParentRectTrs);
				imgs[i] = _sceneEntry.img;
				sceneEntries[i] = _sceneEntry;
			}
			path.onChanged += OnPathChanged;
			pivot.onChanged += OnPivotChanged;
			tint.onChanged += OnTintChanged;
		}

		void OnDestroy ()
		{
			path.onChanged -= OnPathChanged;
			pivot.onChanged -= OnPivotChanged;
			tint.onChanged -= OnTintChanged;
			for (int i = 1; i < imgs.Length; i ++)
			{
				Image img = imgs[i];
				ScenePanel scenePanel = ScenePanel.instances[i];
				scenePanel.entries = scenePanel.entries.Remove(sceneEntries[i]);
				Destroy(img.gameObject);
			}
		}

		void OnPathChanged ()
		{
			string imgPath = Path.Combine(Application.dataPath, path.val);
			if (File.Exists(imgPath))
			{
				ImageConversion.LoadImage(tex, File.ReadAllBytes(Path.GetFullPath(imgPath)));
				Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot.val, 1);
				for (int i = 0; i < imgs.Length; i ++)
				{
					Image img = imgs[i];
					img.sprite = sprite;
				}
			}
			else
				for (int i = 0; i < imgs.Length; i ++)
				{
					Image img = imgs[i];
					img.sprite = null;
				}
		}

		void OnPivotChanged ()
		{
			Vector2 anchoredPos = sceneEntry.rectTrs.anchoredPosition;
			for (int i = 0; i < sceneEntries.Length; i ++)
			{
				SceneEntry sceneEntry = sceneEntries[i];
				RectTransform rectTrs = sceneEntry.rectTrs;
				rectTrs.pivot = pivot.val;
				rectTrs.anchoredPosition = anchoredPos;
			}
		}

		void OnTintChanged ()
		{
			for (int i = 0; i < imgs.Length; i ++)
			{
				Image img = imgs[i];
				img.color = tint.val;
			}
		}

		[Serializable]
		public class Data : _Component.Data
		{
			public string path;
			public _Vector2 pivot;
			public _Vector4 tint;

			public override void Set (_Component component)
			{
				base.Set (component);
				_Image img = (_Image) component;
				path = img.path.val;
				pivot = _Vector2.FromVec2(img.pivot.val);
				tint = _Vector4.FromColor(img.tint.val);
			}

			public override void Apply (_Component component)
			{
				base.Apply (component);
				_Image img = (_Image) component;
				img.path.Set (path);
				img.pivot.Set (pivot.ToVec2());
				img.tint.Set (tint.ToColor());
			}
		}
	}
}