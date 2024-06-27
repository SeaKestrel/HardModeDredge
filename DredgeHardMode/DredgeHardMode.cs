using System;
using UnityEngine;
using Winch.Core;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

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
        /// This function loads configuration of the save into this object
        /// </summary>
        public void Load()
        {
            if (GameManager.Instance.SaveData.GetIntVariable("Delay", -1) == -1) // If the Delay does not exists
            {
                this.Delay = 60;
                GameManager.Instance.SaveData.SetIntVariable("Delay", this.Delay);
                WinchCore.Log.Info("Delay does not exists for this save. Creating...");
            }
            else // If the delay exists
            {
                this.Delay = GameManager.Instance.SaveData.GetIntVariable("Delay", -1);
            }

            if (GameManager.Instance.SaveData.GetIntVariable("DailyDecrease", -1) == -1) // If the DailyDecrease does not exists
            {
                this.DailyDecrease = 5;
                GameManager.Instance.SaveData.SetIntVariable("DailyDecrease", this.DailyDecrease);
                WinchCore.Log.Info("DailyDecrease does not exists for this save. Creating...");
            }
            else
            {
                this.Delay = GameManager.Instance.SaveData.GetIntVariable("Delay", -1);
            }

            if (GameManager.Instance.SaveData.GetIntVariable("MinimumSpawnInterval", -1) == -1) // If the MinimumSpawnInterval does not exists
            {
                this.MinimumSpawnInterval = 10;
                GameManager.Instance.SaveData.SetIntVariable("MinimumSpawnInterval", this.MinimumSpawnInterval);
                WinchCore.Log.Info("MinimumSpawnInterval does not exists for this save. Creating...");
            }
            else
            {
                this.Delay = GameManager.Instance.SaveData.GetIntVariable("Delay", -1);
            }
        }

        /// <summary>
        /// This function saves the config in the DREDGE save
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
        public bool IsGameStarted = false;
        public bool ShouldBeHard = false;
        public int i = 0;
        public Config Config = new Config();
        private static System.Random rnd = new System.Random();
        public UnityAction buttonClickAction = StartNewSave;

        public void Awake()
		{
            Instance = this;

            WinchCore.Log.Info("Adding OnGameStarted handler");
			GameManager.Instance.OnGameStarted += OnGameStarted;

            WinchCore.Log.Info("Adding OnGameEnded handler");
            GameManager.Instance.OnGameEnded += OnGameEnded;

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
        /// This function spawns an event
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

            i = 0; // Resetting the timer to 0
        }

        /// <summary>
        /// Handler for OnGameStarted event
        /// </summary>
		private void OnGameStarted()
		{
            if (!ShouldBeHard) return; // If this save shouldn't be hardmode, pass
            if (ShouldBeHard && GameManager.Instance.SaveData.GetBoolVariable("hardmode") == false)
            {
                GameManager.Instance.SaveData.SetBoolVariable("hardmode", true);
            }

            IsGameStarted = true;

            WinchCore.Log.Info("Adding OnDayChanged handler");
            GameEvents.Instance.OnDayChanged += DayChangedEvent;

            if(ShouldBeHard && GameManager.Instance.SaveData.GetBoolVariable("hardmode") == false)
            {
                GameManager.Instance.SaveData.SetBoolVariable("hardmode", true);
            }

            InvokeRepeating("SpawnEvent", 0, 1f); // Starting the events
		}

        private void OnGameEnded()
        {
            ShouldBeHard = IsGameStarted = false;
        }

        public static void StartNewSave()
        {
            WinchCore.Log.Error("Button clicked!");
        }
	}
}
