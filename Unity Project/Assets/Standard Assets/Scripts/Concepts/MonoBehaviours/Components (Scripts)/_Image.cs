using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Image : _Component
	{
		public Image img;
		public StringValue path;
		public Vector2Value pivot;
		Texture2D tex;

		void Awake ()
		{
			img.rectTransform.SetParent(EternityEngine.instance.canvasRectTrs);
			tex = new Texture2D(1, 1);
			path.onChanged += () => {
				string imgPath = Path.Combine(Application.dataPath, path.val);
				if (File.Exists(imgPath))
				{
					ImageConversion.LoadImage(tex, File.ReadAllBytes(Path.GetFullPath(imgPath)));
					img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot.val, 1);
				}
				else
					img.sprite = null;
			};
			pivot.onChanged += () => {
				RectTransform recTrs = img.rectTransform;
				Vector2 anchoredPos = recTrs.anchoredPosition; 
				recTrs.pivot = pivot.val;
				recTrs.anchoredPosition = anchoredPos;
			};
		}

		void OnDestroy ()
		{
			path.onChanged -= () => {
				string imgPath = Path.Combine(Application.dataPath, path.val);
				if (File.Exists(imgPath))
				{
					ImageConversion.LoadImage(tex, File.ReadAllBytes(Path.GetFullPath(imgPath)));
					img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot.val, 1);
				}
				else
					img.sprite = null;
			};
			pivot.onChanged -= () => {
				RectTransform recTrs = img.rectTransform;
				Vector2 anchoredPos = recTrs.anchoredPosition; 
				recTrs.pivot = pivot.val;
				recTrs.anchoredPosition = anchoredPos;
			};
		}
	}
}