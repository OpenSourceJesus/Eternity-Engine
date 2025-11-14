using System.IO;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Image : _Component
	{
		public Image img;
		[HideInInspector]
		public Image[] imgs = new Image[0];
		public StringValue path;
		public Vector2Value pivot;
		Texture2D tex;

		public override void Start ()
		{
			base.Start ();
			img.rectTransform.SetParent(ScenePanel.instances[0].obsParentRectTrs);
			imgs = new Image[ScenePanel.instances.Length];
			imgs[0] = img;
			for (int i = 1; i < ScenePanel.instances.Length; i ++)
			{
				ScenePanel scenePanel = ScenePanel.instances[i];
				Image _img = Instantiate(img, scenePanel.obsParentRectTrs);
				imgs[i] = _img;
				scenePanel.imgs = scenePanel.imgs.Add(_img);
			}
			tex = new Texture2D(1, 1);
			path.onChanged += OnPathChanged;
			pivot.onChanged += OnPivotChanged;
			ob.imgs = ob.imgs.AddRange(imgs);
		}

		void OnDestroy ()
		{
			path.onChanged -= OnPathChanged;
			pivot.onChanged -= OnPivotChanged;
			for (int i = 1; i < imgs.Length; i ++)
			{
				Image img = imgs[i];
				ScenePanel scenePanel = ScenePanel.instances[i];
				scenePanel.imgs = scenePanel.imgs.Remove(img);
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
			RectTransform recTrs = img.rectTransform;
			Vector2 anchoredPos = recTrs.anchoredPosition;
			for (int i = 0; i < imgs.Length; i ++)
			{
				Image img = imgs[i];
				recTrs.pivot = pivot.val;
				recTrs.anchoredPosition = anchoredPos;
			}
		}
	}
}