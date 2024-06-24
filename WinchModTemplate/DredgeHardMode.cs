using UnityEngine;
using Winch.Core;

namespace DredgeHardMode
{
	public class DredgeHardMode : MonoBehaviour
	{
		public bool IsGameStarted;

        private static System.Random rnd = new System.Random();

        public void Awake()
		{

			WinchCore.Log.Info("Adding OnGameStarted handler");
			GameManager.Instance.OnGameStarted += OnGameStarted;

            WinchCore.Log.Debug($"{nameof(DredgeHardMode)} has loaded!");
        }

		void SpawnEvent()
		{
            int index = rnd.Next(GameManager.Instance.DataLoader.allWorldEvents.Count);
            WorldEventData worldEvent = GameManager.Instance.DataLoader.allWorldEvents[index];
            WinchCore.Log.Debug($"Spawning event No. {index}: {worldEvent.name}");
            GameManager.Instance.WorldEventManager.DoEvent(worldEvent);

            GameManager.Instance.UI.ShowNotificationWithColor(NotificationType.SPOOKY_EVENT, "notification.disaster-button", GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL));
        }

		private void OnGameStarted()
		{
			IsGameStarted = true;
			InvokeRepeating("SpawnEvent", 0, 10f);
		}
	}
}
