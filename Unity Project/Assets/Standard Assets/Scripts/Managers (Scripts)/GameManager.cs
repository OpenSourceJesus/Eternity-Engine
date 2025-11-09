using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EternityEngine
{
	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
		public GameObject[] registeredGos = new GameObject[0];
		static string EnabledGoNamesString
		{
			get
			{
				return SaveAndLoadManager.GetString("Enabled GameObject names");
			}
			set
			{
				SaveAndLoadManager.SetString ("Enabled GameObject names", value);
			}
		}
		static string DisabledGoNamesString
		{
			get
			{
				return SaveAndLoadManager.GetString("Disabled GameObject names");
			}
			set
			{
				SaveAndLoadManager.SetString ("Disabled GameObject names", value);
			}
		}
		public GameModifier[] gameModifiers = new GameModifier[0];
		public float timeSpeed;
		public TemporaryActiveText notificationTemporaryActiveText;
		public GameObject achieveNotificationGo;
		public Image achieveNotificationImage;
		public TMP_Text achieveNotificationText;
		public Transform bossHealthBarTrs;
		public _ParticleSystem bloodPrefab;
		public static Dictionary<string, GameModifier> gameModifierDict = new Dictionary<string, GameModifier>();
		public static bool paused;
		public static IUpdatable[] updatables = new IUpdatable[0];
		public static uint framesSinceLevelLoaded;
		public static bool isQuittingGame;
		public static float pausedTime;
		public static float TimeSinceLevelLoad
		{
			get
			{
				return Time.timeSinceLevelLoad - pausedTime;
			}
		}
		public const int LAGGY_FRAMES_ON_LOAD_SCENE = 2;
		static bool initialized;
		const string STRING_SEPERATOR = "|";

		public override void Awake ()
		{
			base.Awake ();
			if (!initialized)
			{
#if !UNITY_WEBGL
				SaveAndLoadManager.Init ();
#endif
#if UNITY_EDITOR
				paused = false;
				isQuittingGame = false;
#endif
				initialized = true;
			}
			SetGameObjectsActive ();
			if (instance != this)
				return;
			gameModifierDict.Clear();
			for (int i = 0; i < gameModifiers.Length; i ++)
			{
				GameModifier gameModifier = gameModifiers[i];
				gameModifierDict.Add(gameModifier.name, gameModifier);
			}
			SceneManager.sceneLoaded += OnSceneLoaded;
			QualitySettings.globalTextureMipmapLimit = 0;
			timeSpeed = SettingsMenu.TimeSpeed;
			SaveAndLoadManager.Save ();
		}

		void Update ()
		{
			if (framesSinceLevelLoaded == 1 && achieveNotificationGo != null)
				achieveNotificationGo.SetActive(false);
			else if (framesSinceLevelLoaded > LAGGY_FRAMES_ON_LOAD_SCENE)
			{
				Physics2D.Simulate(Time.deltaTime);
				Physics2D.SyncTransforms();
			}
			for (int i = 0; i < updatables.Length; i ++)
			{
				IUpdatable updatable = updatables[i];
				updatable.DoUpdate ();
			}
			if (ObjectPool.Instance != null && ObjectPool.instance.enabled)
				ObjectPool.instance.DoUpdate ();
			InputSystem.Update ();
			if (paused)
				pausedTime += Time.unscaledDeltaTime;
			framesSinceLevelLoaded ++;
		}

		void OnDestroy ()
		{
			if (instance == this)
				SceneManager.sceneLoaded -= OnSceneLoaded;
		}
		
		void OnSceneLoaded (Scene scene = new Scene(), LoadSceneMode loadMode = LoadSceneMode.Single)
		{
			Instance.StopAllCoroutines();
			framesSinceLevelLoaded = 0;
			pausedTime = 0;
		}

		public void DisplayNotification (string text)
		{
			notificationTemporaryActiveText.text.text = text;
			notificationTemporaryActiveText.Do ();
		}

		public void ToggleGameObject (GameObject go)
		{
			go.SetActive(!go.activeSelf);
		}

		public static void SetGameObjectsActive ()
		{
			string[] stringSeperators = { STRING_SEPERATOR };
			string[] enabledGoNames = EnabledGoNamesString.Split(stringSeperators, StringSplitOptions.None);
			List<GameObject> registeredGosRemaining = new List<GameObject>(Instance.registeredGos);
			for (int i = 0; i < enabledGoNames.Length; i ++)
			{
				string goName = enabledGoNames[i];
				for (int i2 = 0; i2 < registeredGosRemaining.Count; i2 ++)
				{
					GameObject registeredGo = registeredGosRemaining[i2];
					if (goName == registeredGo.name)
					{
						registeredGo.SetActive(true);
						registeredGosRemaining.RemoveAt(i2);
						break;
					}
				}
			}
			string[] disabledGoNames = DisabledGoNamesString.Split(stringSeperators, StringSplitOptions.None);
			for (int i = 0; i < disabledGoNames.Length; i ++)
			{
				string goName = disabledGoNames[i];
				GameObject go = GameObject.Find(goName);
				if (go != null)
					go.SetActive(false);
			}
		}
		
		public static void ActivateGameObjectForever (GameObject go)
		{
			go.SetActive(true);
			ActivateGameObjectForever (go.name);
		}
		
		public static void DeactivateGameObjectForever (GameObject go)
		{
			go.SetActive(false);
			DeactivateGameObjectForever (go.name);
		}
		
		public static void ActivateGameObjectForever (string goName)
		{
			DisabledGoNamesString = DisabledGoNamesString.Replace(STRING_SEPERATOR + goName, "");
			if (!EnabledGoNamesString.Contains(goName))
				EnabledGoNamesString += STRING_SEPERATOR + goName;
		}
		
		public static void DeactivateGameObjectForever (string goName)
		{
			EnabledGoNamesString = EnabledGoNamesString.Replace(STRING_SEPERATOR + goName, "");
			if (!DisabledGoNamesString.Contains(goName))
				DisabledGoNamesString += STRING_SEPERATOR + goName;
		}

		public static void SetPaused (bool pause)
		{
			paused = pause;
			Time.timeScale = instance.timeSpeed * (1 - pause.GetHashCode());
		}

		public void Quit ()
		{
			Application.Quit();
		}

		void OnApplicationQuit ()
		{
			isQuittingGame = true;
			SaveAndLoadManager.Save ();
#if UNITY_EDITOR
			initialized = false;
#endif
		}

		public static void Log (object obj)
		{
			print(obj);
		}

		public static void DestroyImmediate (Object obj)
		{
			Object.DestroyImmediate(obj);
		}
		
#if UNITY_EDITOR
		public static void DestroyOnNextEditorUpdate (Object obj)
		{
			EditorApplication.update += () => { if (obj == null) return; DestroyObject (obj); };
		}

		static void DestroyObject (Object obj)
		{
			if (obj == null)
				return;
			EditorApplication.update -= () => { DestroyObject (obj); };
			DestroyImmediate(obj);
		}
#endif
		
		public static bool ModifierExistsAndIsActive (string name)
		{
			GameModifier gameModifier;
			if (gameModifierDict.TryGetValue(name, out gameModifier))
				return gameModifier.isActive;
			else
				return false;
		}

		public static bool ModifierIsActive (string name)
		{
			return gameModifierDict[name].isActive;
		}

		public static bool ModifierExists (string name)
		{
			return gameModifierDict.ContainsKey(name);
		}

		[Serializable]
		public class GameModifier
		{
			public string name;
			public bool isActive;
		}
	}
}