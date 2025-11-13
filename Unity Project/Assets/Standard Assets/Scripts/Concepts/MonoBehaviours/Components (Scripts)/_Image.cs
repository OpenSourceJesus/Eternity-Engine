using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace EternityEngine
{
	public class _Image : _Component
	{
		public Image img;
		public StringValue path;
		Texture2D tex;

		void Awake ()
		{
			img.rectTransform.SetParent(EternityEngine.instance.canvasRectTrs);
			tex = new Texture2D(1, 1);
			path.onChanged += () => {
				if (File.Exists(path.val))
				{
					ImageConversion.LoadImage(tex, File.ReadAllBytes(path.val));
					img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2, 1);
				}
			};
		}

		void OnDestroy ()
		{
			path.onChanged -= () => {
				if (File.Exists(path.val))
				{
					ImageConversion.LoadImage(tex, File.ReadAllBytes(path.val));
					img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2, 1);
				}
			};
		}
	}
}