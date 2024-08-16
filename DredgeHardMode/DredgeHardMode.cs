using System;
using UnityEngine;
using Winch.Core;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using InControl;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using Winch.Util;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace DredgeHardMode
{
    public class CounterLabel : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI textField;

        private float timeUntilUpdate;

        private float timeBetweenUpdatesSec = 0.25f;

        private void Update()
        {
            timeUntilUpdate -= Time.deltaTime;
            if (timeUntilUpdate <= 0f)
            {
                timeUntilUpdate = timeBetweenUpdatesSec;
                textField.text = LocalizationSettings.StringDatabase.GetLocalizedString(LanguageManager.STRING_TABLE, "time-left", null, FallbackBehavior.UseProjectSettings) + " <mspace=13>00</mspace>:<mspace=13>" + (DredgeHardMode.Instance.Config.Delay - DredgeHardMode.Instance.i).ToString("00") + "</mspace>";
            }
        }
    }

    public class Config
    {
        public int Delay;
        public int DailyDecrease;
        public int MinimumSpawnInterval;

        public Config() {}

        /// <summary>
        /// Loads DREDGE save in the config
        /// </summary>
        public void Load()
        {
            WinchCore.Log.Debug("Loading config");
            if (GameManager.Instance.SaveData.GetIntVariable("Delay", -1) == -1) // If the Delay does not exists
            {
                Delay = 60;
                GameManager.Instance.SaveData.SetIntVariable("Delay", Delay);
                WinchCore.Log.Debug("Delay does not exists for this save. Creating...");
            }
            else // If the delay exists
            {
                WinchCore.Log.Debug("Delay existing. Loading it");
                Delay = GameManager.Instance.SaveData.GetIntVariable("Delay", -1);
            }

            if (GameManager.Instance.SaveData.GetIntVariable("DailyDecrease", -1) == -1) // If the DailyDecrease does not exists
            {
                DailyDecrease = 5;
                GameManager.Instance.SaveData.SetIntVariable("DailyDecrease", DailyDecrease);
                WinchCore.Log.Debug("DailyDecrease does not exists for this save. Creating...");
            }
            else
            {
                WinchCore.Log.Debug("DailyDecrease existing. Loading it");
                DailyDecrease = GameManager.Instance.SaveData.GetIntVariable("DailyDecrease", -1);
            }

            if (GameManager.Instance.SaveData.GetIntVariable("MinimumSpawnInterval", -1) == -1) // If the MinimumSpawnInterval does not exists
            {
                MinimumSpawnInterval = 10;
                GameManager.Instance.SaveData.SetIntVariable("MinimumSpawnInterval", MinimumSpawnInterval);
                WinchCore.Log.Debug("MinimumSpawnInterval does not exists for this save. Creating...");
            }
            else
            {
                WinchCore.Log.Debug("MinimumSpawnInterval existing. Loading it");
                MinimumSpawnInterval = GameManager.Instance.SaveData.GetIntVariable("MinimumSpawnInterval", -1);
            }
        }

        /// <summary>
        /// Saves the config in the DREDGE save
        /// </summary>
        public void Save()
        {
            WinchCore.Log.Debug("Saving config");
            GameManager.Instance.SaveData.SetIntVariable("Delay", Delay);
            GameManager.Instance.SaveData.SetIntVariable("DailyDecrease", DailyDecrease);
            GameManager.Instance.SaveData.SetIntVariable("MinimumSpawnInterval", MinimumSpawnInterval);
        }
    }

    public class DredgeHardMode : MonoBehaviour
    {

        public static DredgeHardMode Instance;

        public GameObject Counter;
        public bool IsGameStarted = false;
        public bool ShouldBeHard = false;
        public bool died = false;
        public int i = 0;
        public Config Config = new Config();
        private static System.Random rnd = new System.Random();
        public UnityAction shouldBeHardAction = ShouldBeHardAction;
        public List<WorldEventData> worldEventDatas = new List<WorldEventData>();

        public void Awake()
        {
            Instance = this;

            WinchCore.Log.Info("Adding OnGameEnded handler");
            GameManager.Instance.OnGameEnded += OnGameEnded;

            WinchCore.Log.Debug($"{nameof(DredgeHardMode)} has loaded!");
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                SpawnEvent();
            }
        }

        /// <summary>
        /// Handler for OnDayChanged event
        /// </summary>
        /// <param name="day"></param>
        public void DayChangedEvent(int day)
        {
            if (Config.Delay >= Config.MinimumSpawnInterval + Config.DailyDecrease) Config.Delay -= Config.DailyDecrease; // If the Delay is still greater than the minimum delay, we decrease the delay
        }

        /// <summary>
        /// Spawns an random event
        /// </summary>
		void ShowEvent()
        {
            if (GameManager.Instance.Player.IsDocked) return; // If the player is docked, we don't want to execute anything

            if (i < Config.Delay) { i++; return; } // If the delay isn't reached yet, continue

            SpawnEvent();

            i = 0; // Resetting the timer to 0
        }

        /// <summary>
        /// This function spawns an random event
        /// </summary>
        public void SpawnEvent()
        {
            // Code from DesasterButton by Hacktix
            int index = rnd.Next(worldEventDatas.Count);
            WorldEventData worldEvent = worldEventDatas[index];

            WinchCore.Log.Debug($"Spawning event No. {index}: {worldEvent.name}");
            GameManager.Instance.WorldEventManager.DoEvent(worldEvent);

            GameEvents.Instance.TriggerNotification(NotificationType.SPOOKY_EVENT, "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL) + ">" + ParseAllKey("event_notification", worldEvent.name) + "</color>");
        }

        /// <summary>
        /// Handler for OnGameStarted event
        /// </summary>
        public void OnGameStarted()
        {
            if (GameManager.Instance.SaveData.GetBoolVariable("hardmode") == false)
            {
                GameManager.Instance.SaveData.SetBoolVariable("hardmode", true);
            }

            IsGameStarted = true;

            WinchCore.Log.Debug("Adding OnDayChanged handler");
            GameEvents.Instance.OnDayChanged += DayChangedEvent;

            /*
            * Setting up the Counter
            */
            SetupCounter();

            LoadEvents();

            InvokeRepeating("ShowEvent", 0, 1f); // Starting the events
        }

        /// <summary>
        /// Handler for OnGameEnded event
        /// </summary>
        private void OnGameEnded()
        {
            IsGameStarted = false;
            GameEvents.Instance.OnDayChanged -= DayChangedEvent;
            CancelInvoke("ShowEvent");
        }

        /// <summary>
        /// Action to invoke on the main menu
        /// </summary>
        public static void ShouldBeHardAction()
        {
            WinchCore.Log.Debug("ShouldBeHardAction()");
            DredgeHardMode.Instance.ShouldBeHard = true;
        }

        /// <summary>
        /// This function parse all keys of a phrase for localization
        /// </summary>
        /// <param name="phrase">list of string to parse with locales</param>
        /// <returns></returns>
        public static string ParseAllKey(params string[] phrase)
        {
            string final = "";

            foreach (string k in phrase)
            {
                final += " " + LocalizationSettings.StringDatabase.GetLocalizedString(LanguageManager.STRING_TABLE, k, null, FallbackBehavior.UseProjectSettings);
            }

            return final;
        }

        /// <summary>
        /// This function sets up a counter
        /// </summary>
        private void SetupCounter()
        {
            try
            {
                Counter = Instantiate(GameObject.Find("GameCanvases/GameCanvas/TopPanel/Time/TimeText"), GameObject.Find("GameCanvases/GameCanvas").transform);

                RectTransform rectTransform = Counter.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                rectTransform.sizeDelta = new Vector2(400, 150);

                // Label modifier
                Destroy(Counter.GetComponent<TimeLabel>());
                CounterLabel counterLabel = Counter.AddComponent<CounterLabel>();
                counterLabel.textField = Counter.GetComponent<TextMeshProUGUI>();
                counterLabel.transform.position = new Vector3(1390f, 1032.313f, 0);
                counterLabel.textField.fontSizeMax = 35;

                DontDestroyOnLoad(Counter);
                Counter.SetActive(true);
            }
            catch (Exception ex)
            {
                WinchCore.Log.Error(ex);
            }
        }

        /// <summary>
        /// This functions takes all the events that exists, modify them and take only wanted ones
        /// </summary>
        private void LoadEvents()
        {
            foreach (WorldEventData data in GameManager.Instance.DataLoader.allWorldEvents)
            {
                data.allowInPassiveMode = true;
                data.maxSanity = 1f;
                data.minSanity = 0f;
                if (data.name != "GhostBoat_Pirate" && data.name != "GhostBoat_Player1" && data.name != "GhostBoat_Player2" && data.name != "GhostBoat_Player3")
                {
                    worldEventDatas.Add(data);
                    WinchCore.Log.Debug($"Adding event {data.name} to worldEventDatas");
                }
            }
        }

    }
}