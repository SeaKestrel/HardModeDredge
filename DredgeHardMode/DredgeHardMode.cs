using System;
using UnityEngine;
using Winch.Core;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

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
                textField.text = "Time before event: <mspace=13>00</mspace>:<mspace=13>" + (DredgeHardMode.Instance.Config.Delay - DredgeHardMode.Instance.i) + "</mspace>";
            }
        }
    }

    public class Config
    {
        public int Delay;
        public int DailyDecrease;
        public int MinimumSpawnInterval;

        public Config()
        {

        }

        /// <summary>
        /// Loads DREDGE save in the config
        /// </summary>
        public void Load()
        {
            if (GameManager.Instance.SaveData.GetIntVariable("Delay", -1) == -1) // If the Delay does not exists
            {
                Delay = 60;
                GameManager.Instance.SaveData.SetIntVariable("Delay", Delay);
                WinchCore.Log.Info("Delay does not exists for this save. Creating...");
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
                WinchCore.Log.Info("DailyDecrease does not exists for this save. Creating...");
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
                WinchCore.Log.Info("MinimumSpawnInterval does not exists for this save. Creating...");
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
            GameManager.Instance.SaveData.SetIntVariable("Delay", Delay);
            GameManager.Instance.SaveData.SetIntVariable("DailyDecrease", DailyDecrease);
            GameManager.Instance.SaveData.SetIntVariable("MinimumSpawnInterval", MinimumSpawnInterval);
        }
    }

	public class DredgeHardMode : MonoBehaviour
	{

        public static DredgeHardMode Instance;

        public GameObject Counter;
		public bool IsGameStarted;
        public int i = 0;
        public Config Config = new Config();
        private static System.Random rnd = new System.Random();

        public void Awake()
		{
            Instance = this;

            WinchCore.Log.Info("Adding OnGameStarted handler");
			GameManager.Instance.OnGameStarted += OnGameStarted;

            WinchCore.Log.Debug($"{nameof(DredgeHardMode)} has loaded!");
        }

        /// <summary>
        /// Handler for OnDayChanged event
        /// </summary>
        /// <param name="day"></param>
        public void DayChangedEvent(int day)
        {
            if (Config.Delay > Config.MinimumSpawnInterval + Config.DailyDecrease) Config.Delay -= Config.DailyDecrease; // If the Delay is still greater than the minimum delay, we decrease the delay
        }

        /// <summary>
        /// Spawns an random event
        /// </summary>
		void SpawnEvent()
		{
			if (GameManager.Instance.Player.IsDocked) return; // If the player is docked, we don't want to execute anything

            if (i < Config.Delay) { i++; return; } // If the delay isn't reached yet, continue

            // Code from DesasterButton by Hacktix
            int index = rnd.Next(GameManager.Instance.DataLoader.allWorldEvents.Count);
            WorldEventData worldEvent = GameManager.Instance.DataLoader.allWorldEvents[index];
            WinchCore.Log.Debug($"Spawning event No. {index}: {worldEvent.name}");
            GameManager.Instance.WorldEventManager.DoEvent(worldEvent);

            GameManager.Instance.UI.ShowNotificationWithColor(NotificationType.SPOOKY_EVENT, "notification.disaster-button", GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL));

            i = 0;
        }

        /// <summary>
        /// Handler for OnGameStarted event
        /// </summary>
		private void OnGameStarted()
		{
            IsGameStarted = true;

            WinchCore.Log.Info("Adding OnDayChanged handler");
            GameEvents.Instance.OnDayChanged += DayChangedEvent;

            /*
             * Setting up the Counter
             */
            try
            {
                Counter = Instantiate(GameObject.Find("GameCanvases/GameCanvas/TopPanel/Time/TimeText"), GameObject.Find("GameCanvases/GameCanvas").transform);

                RectTransform rectTransform = Counter.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                rectTransform.sizeDelta = new Vector2(300, 150);

                // Label modifier
                Destroy(Counter.GetComponent<TimeLabel>());
                CounterLabel counterLabel = Counter.AddComponent<CounterLabel>();
                counterLabel.textField = Counter.GetComponent<TextMeshProUGUI>();
                counterLabel.transform.position = new Vector3(1362.549f, 1032.313f, 0);

                DontDestroyOnLoad(Counter);
                Counter.SetActive(true);
            } catch (Exception ex)
            {
                WinchCore.Log.Error(ex);
            }

            InvokeRepeating("SpawnEvent", 0, 1f);
		}
	}
}
