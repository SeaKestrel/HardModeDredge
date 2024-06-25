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

        public void CheckIntegrity()
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
    }

	public class DredgeHardMode : MonoBehaviour
	{

        public static DredgeHardMode Instance;

        public GameObject Counter;

		public bool IsGameStarted;

        public int i = 0;

        public Config Config = new Config();

        //public Dictionary<string, object> Config;

        private static System.Random rnd = new System.Random();

        public void Awake()
		{
            Instance = this;

            WinchCore.Log.Info("Adding OnGameStarted handler");
			GameManager.Instance.OnGameStarted += OnGameStarted;

            WinchCore.Log.Info("Adding OnGameEnded handler");
            GameManager.Instance.OnGameEnded += OnGameEnded;

            /*try
            {
                if (!File.Exists("Mods/DaSea.DredgeHardMode/config.json"))
                {
                    Config = new Config() { DailyDecrease = 5, Delay = 60, MinimumSpawnInterval = 10 };
                    File.Create("Mods/DaSea.DredgeHardMode/config.json");
                    File.WriteAllText("Mods/DaSea.DredgeHardMode/config.json", JsonConvert.SerializeObject(Config));
                }
                else
                {
                    string config = File.ReadAllText("Mods/DaSea.DredgeHardMode/config.json");
                    Config = JsonConvert.DeserializeObject<Config>(config) ?? throw new InvalidOperationException("Unable to parse config.json file.");
                }
            }
            catch (Exception ex)
            {
                WinchCore.Log.Error(ex);
            }*/

            WinchCore.Log.Debug($"{nameof(DredgeHardMode)} has loaded!");
        }

        public void DayChangedEvent(int day)
        {
            if (Config.Delay > Config.MinimumSpawnInterval + Config.Delay) Config.Delay -= Config.DailyDecrease; // If the Delay is still greater than the minimum delay, we decrease the delay
        }

        public void QuestCompleteEvent(QuestData data)
        {
        }

        public void PlayerDockedEvent(Dock dock)
        {
             GameManager.Instance.SaveData.SetIntVariable("Delay", Config.Delay);
             GameManager.Instance.SaveData.SetIntVariable("DailyDecrease", Config.DailyDecrease);
             GameManager.Instance.SaveData.SetIntVariable("MinimumSpawnInterval", Config.MinimumSpawnInterval);
        }

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

		private void OnGameStarted()
		{
            if (GameManager.Instance.SaveData.GetIntVariable("Hardmode", -1) == -1) return;

            IsGameStarted = true;

            WinchCore.Log.Info("Adding OnDayChanged handler");
            GameEvents.Instance.OnDayChanged += DayChangedEvent;

            // GameEvents.Instance.OnQuestCompleted += QuestCompleteEvent;
            GameEvents.Instance.OnPlayerDockedToggled += PlayerDockedEvent;

            try
            {
                Config.CheckIntegrity();
            }
            catch (Exception e)
            {
                WinchCore.Log.Error(e);
            }

            try
            {
                Counter = Instantiate(GameObject.Find("GameCanvases/GameCanvas/TopPanel/Time/TimeText"), GameObject.Find("GameCanvases/GameCanvas").transform);
                Counter.SetActive(true);
                Destroy(Counter.GetComponent<TimeLabel>());

                CounterLabel counterLabel = Counter.AddComponent<CounterLabel>();
                counterLabel.textField = Counter.GetComponent<TextMeshProUGUI>();

                counterLabel.transform.position = new Vector3(1362.549f, 1032.313f, 0);

                DontDestroyOnLoad(Counter);
            } catch (Exception ex)
            {
                WinchCore.Log.Error(ex);
            }

            InvokeRepeating("SpawnEvent", 0, 1f);
		}

        private void OnGameEnded()
        {
            GameManager.Instance.SaveData.SetIntVariable("Delay", -1);
        }
	}
}
