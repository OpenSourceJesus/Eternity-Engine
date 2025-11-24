using System;
using System.IO;
using Extensions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace EternityEngine
{
	public class SaveAndLoadManager : SingletonMonoBehaviour<SaveAndLoadManager>
	{
		public static SaveData saveData = new SaveData();
		public static bool isLoading;

		public static void Init ()
		{
#if !UNITY_WEBGL
			saveData.assetsDatasDict = new Dictionary<string, Asset.Data>();
			saveData.boolDict = new Dictionary<string, bool>();
			saveData.intDict = new Dictionary<string, int>();
			saveData.floatDict = new Dictionary<string, float>();
			saveData.stringDict = new Dictionary<string, string>();
			saveData.vector2Dict = new Dictionary<string, _Vector2>();
			saveData.boolArrayDict = new Dictionary<string, bool[]>();
			saveData.byteArrayDict = new Dictionary<string, byte[]>();
			saveData.vector2IntArrayDict = new Dictionary<string, _Vector2Int[]>();
			string autoSaveFilePath = Path.Combine(Application.dataPath, "Auto Save.txt");
			if (File.Exists(autoSaveFilePath))
				Load (autoSaveFilePath);
#endif
		}

		public static void Save (string saveFilePath)
		{
#if !UNITY_WEBGL
			GameManager.assets.Sort(new AssetComperer());
			for (int i = 0; i < GameManager.assets.Count; i ++)
			{
				Asset asset = GameManager.assets[i];
				asset.SetData ();
				saveData.assetsDatasDict[asset.id] = asset._Data;
				// print(asset.name + " : " + (byte) asset.id[0]);
			}
			saveData.exportBackgroundColor = _Vector4.FromColor(EternityEngine.instance.backgroundColor.val);
			saveData.useGravity = EternityEngine.instance.useGravity.val;
			saveData.gravity = _Vector3.FromVec3(EternityEngine.instance.gravity.val);
			saveData.unitLen = EternityEngine.instance.unitLen.val;
			saveData.exportPath = EternityEngine.instance.exportPath.val;
			saveData.debugMode = EternityEngine.instance.debugMode.val;
			FileStream fileStream = new FileStream(saveFilePath, FileMode.Create);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(fileStream, saveData);
			fileStream.Close();
#endif
		}

		public static void Load (string saveFilePath)
		{
#if !UNITY_WEBGL
			isLoading = true;
			FileStream fileStream = new FileStream(saveFilePath, FileMode.Open);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			saveData = (SaveData) binaryFormatter.Deserialize(fileStream);
			fileStream.Close();
			foreach (KeyValuePair<string, Asset.Data> keyValuePair in saveData.assetsDatasDict)
				if (Asset.Get<Asset>(keyValuePair.Key) == null)
				{
					// print((byte) keyValuePair.Key[0]);
					keyValuePair.Value.GenAsset ();
				}
			EternityEngine.instance.backgroundColor.Set (saveData.exportBackgroundColor.ToColor());
			EternityEngine.instance.useGravity.Set (saveData.useGravity);
			EternityEngine.instance.gravity.Set (saveData.gravity.ToVec3());
			EternityEngine.instance.unitLen.Set (saveData.unitLen);
			EternityEngine.instance.exportPath.Set (saveData.exportPath);
			EternityEngine.instance.debugMode.Set (saveData.debugMode);
			// InspectorPanel.RegenEntries (HierarchyPanel.instances[0].selected.Length > 1);
			isLoading = false;
#endif
		}

		public static bool GetBool (string key, bool value = false)
		{
#if UNITY_WEBGL
			return PlayerPrefs.GetInt(key, value.GetHashCode()) == 1;
#else
			bool output = false;
			if (saveData.boolDict.TryGetValue(key, out output))
				return output;
			else
				return value;
#endif
		}

		public static void SetBool (string key, bool value)
		{
#if UNITY_WEBGL
			PlayerPrefs.SetInt(key, value.GetHashCode());
#else
			saveData.boolDict[key] = value;
#endif
		}

		public static int GetInt (string key, int value = 0)
		{
#if UNITY_WEBGL
			return PlayerPrefs.GetInt(key, value);
#else
			int output = 0;
			if (saveData.intDict.TryGetValue(key, out output))
				return output;
			else
				return value;
#endif
		}

		public static void SetInt (string key, int value)
		{
#if UNITY_WEBGL
			PlayerPrefs.SetInt(key, value);
#else
			saveData.intDict[key] = value;
#endif
		}

		public static float GetFloat (string key, float value = 0)
		{
#if UNITY_WEBGL
			return PlayerPrefs.GetFloat(key, value);
#else
			float output = 0;
			if (saveData.floatDict.TryGetValue(key, out output))
				return output;
			else
				return value;
#endif
		}

		public static void SetFloat (string key, float value)
		{
#if UNITY_WEBGL
			PlayerPrefs.SetFloat(key, value);
#else
			saveData.floatDict[key] = value;
#endif
		}

		public static string GetString (string key, string value = "")
		{
#if UNITY_WEBGL
			return PlayerPrefs.GetString(key, value);
#else
			string output = null;
			if (saveData.stringDict.TryGetValue(key, out output))
				return output;
			else
				return value;
#endif
		}

		public static void SetString (string key, string value)
		{
#if UNITY_WEBGL
			PlayerPrefs.SetString(key, value);
#else
			saveData.stringDict[key] = value;
#endif
		}

		public static _Vector2 GetVector2 (string key, _Vector2 value = new _Vector2())
		{
#if UNITY_WEBGL
			return new _Vector2(PlayerPrefs.GetFloat(key + ".x", value.x), PlayerPrefs.GetFloat(key + ".y", value.y));
#else
			_Vector2 output = new _Vector2();
			if (saveData.vector2Dict.TryGetValue(key, out output))
				return output;
			else
				return value;
#endif
		}

		public static void SetVector2 (string key, _Vector2 value)
		{
#if UNITY_WEBGL
			PlayerPrefs.SetFloat(key + ".x", value.x);
			PlayerPrefs.SetFloat(key + ".y", value.y);
#else
			saveData.vector2Dict[key] = value;
#endif
		}

		public static bool[] GetBoolArray (string key, bool[] values = null)
		{
#if UNITY_WEBGL
			List<bool> output = new List<bool>();
			int index = 0;
			while (PlayerPrefs.HasKey(key + "[" + index + "]"))
			{
				output.Add(PlayerPrefsExtensions.GetBool(key + "[" + index + "]"));
				index ++;
			}
			if (output.Count == 0)
				return values;
			else
				return output.ToArray();
#else
			bool[] output = new bool[0];
			if (saveData.boolArrayDict.TryGetValue(key, out output))
				return output;
			else
				return values;
#endif
		}

		public static void SetBoolArray (string key, bool[] values)
		{
#if UNITY_WEBGL
			for (int i = 0; i < values.Length; i ++)
			{
				bool value = values[i];
				PlayerPrefsExtensions.SetBool (key + "[" + i + "]", value);
			}
			int index = values.Length;
			while (PlayerPrefs.HasKey(key + "[" + index + "]"))
			{
				PlayerPrefs.DeleteKey(key + "[" + index + "]");
				index ++;
			}
#else
			saveData.boolArrayDict[key] = values;
#endif
		}

		public static byte[] GetByteArray (string key, byte[] values = null)
		{
#if UNITY_WEBGL
			List<byte> output = new List<byte>();
			byte index = 0;
			while (PlayerPrefs.HasKey(key + "[" + index + "]"))
			{
				output.Add((byte) PlayerPrefs.GetInt(key + "[" + index + "]"));
				index ++;
			}
			if (output.Count == 0)
				return values;
			else
				return output.ToArray();
#else
			byte[] output = new byte[0];
			if (saveData.byteArrayDict.TryGetValue(key, out output))
				return output;
			else
				return values;
#endif
		}

		public static void SetByteArray (string key, byte[] values)
		{
#if UNITY_WEBGL
			for (byte i = 0; i < values.Length; i ++)
			{
				byte value = values[i];
				PlayerPrefs.SetInt (key + "[" + i + "]", value);
			}
			int index = values.Length;
			while (PlayerPrefs.HasKey(key + "[" + index + "]"))
			{
				PlayerPrefs.DeleteKey(key + "[" + index + "]");
				index ++;
			}
#else
			saveData.byteArrayDict[key] = values;
#endif
		}

		public static _Vector2Int[] GetVector2IntArray (string key, _Vector2Int[] values = null)
		{
#if UNITY_WEBGL
			List<_Vector2Int> output = new List<_Vector2Int>();
			int index = 0;
			while (PlayerPrefs.HasKey(key + "[" + index + "].x"))
			{
				output.Add(new _Vector2Int(PlayerPrefs.GetInt(key + "[" + index + "].x"), PlayerPrefs.GetInt(key + "[" + index + "].y")));
				index ++;
			}
			if (output.Count == 0)
				return values;
			else
				return output.ToArray();
#else
			_Vector2Int[] output = new _Vector2Int[0];
			if (saveData.vector2IntArrayDict.TryGetValue(key, out output))
				return output;
			else
				return values;
#endif
		}

		public static void SetVector2IntArray (string key, _Vector2Int[] values)
		{
#if UNITY_WEBGL
			for (int i = 0; i < values.Length; i ++)
			{
				_Vector2Int value = values[i];
				PlayerPrefs.SetInt(key + "[" + i + "].x", value.x);
				PlayerPrefs.SetInt(key + "[" + i + "].y", value.y);
			}
			int index = values.Length;
			while (PlayerPrefs.HasKey(key + "[" + index + "].x"))
			{
				PlayerPrefs.DeleteKey(key + "[" + index + "].x");
				PlayerPrefs.DeleteKey(key + "[" + index + "].y");
				index ++;
			}
#else
			saveData.vector2IntArrayDict[key] = values;
#endif
		}

		public static void DeleteKey (string key)
		{
#if UNITY_WEBGL
			PlayerPrefs.DeleteKey(key);
#else
			if (saveData.boolDict.Remove(key))
				return;
			else if (saveData.intDict.Remove(key))
				return;
			else if (saveData.floatDict.Remove(key))
				return;
			else if (saveData.stringDict.Remove(key))
				return;
			else if (saveData.vector2Dict.Remove(key))
				return;
			else if (saveData.boolArrayDict.Remove(key))
				return;
			else if (saveData.byteArrayDict.Remove(key))
				return;
			else
				saveData.vector2IntArrayDict.Remove(key);
#endif
		}

		public static void DeleteAll ()
		{
#if UNITY_WEBGL
			PlayerPrefs.DeleteAll();
#else
			saveData.boolDict.Clear();
			saveData.intDict.Clear();
			saveData.floatDict.Clear();
			saveData.stringDict.Clear();
			saveData.vector2Dict.Clear();
			saveData.boolArrayDict.Clear();
			saveData.byteArrayDict.Clear();
			saveData.vector2IntArrayDict.Clear();
			// Save ();
#endif
		}

		public class AssetComperer : IComparer<Asset>
		{
			public int Compare (Asset asset, Asset asset2)
			{
				bool assetIsOb = asset is _Object;
				bool asset2IsOb = asset2 is _Object;
				if (assetIsOb && !asset2IsOb)
					return -1;
				else if (!assetIsOb && asset2IsOb)
					return 1;
				else
					return 0;
			}
		}

		[Serializable]
		public struct SaveData
		{
			public Dictionary<string, Asset.Data> assetsDatasDict;
			public _Vector4 exportBackgroundColor;
			public bool useGravity;
			public _Vector3 gravity;
			public float unitLen;
			public string exportPath;
			public bool debugMode;
			public Dictionary<string, bool> boolDict;
			public Dictionary<string, int> intDict;
			public Dictionary<string, float> floatDict;
			public Dictionary<string, string> stringDict;
			public Dictionary<string, _Vector2> vector2Dict;
			public Dictionary<string, bool[]> boolArrayDict;
			public Dictionary<string, byte[]> byteArrayDict;
			public Dictionary<string, _Vector2Int[]> vector2IntArrayDict;
		}
	}
}