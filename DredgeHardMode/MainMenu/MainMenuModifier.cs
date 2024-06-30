using UnityEngine;
using UnityEngine.Events;
using Winch.Core;
using static UnityEngine.UI.Button;

namespace DredgeHardMode.MainMenu
{
    [AddToMainMenuScene]
    internal class MainMenuModifier : MonoBehaviour
    {
        public void Awake()
        {
            GameObject continueButton = GameObject.Find("Canvases/MenuCanvas/ButtonContainer/").transform.GetChild(0).gameObject;
            ContinueOrNewButton continueOrNewButton = continueButton.GetComponent<ContinueOrNewButton>();
            if (continueOrNewButton.currentMode == ContinueOrNewButton.StartButtonMode.NEW)
            {
                continueButton.GetComponent<BasicButton>().onClick.AddListener(DredgeHardMode.Instance.buttonClickAction);
            }
            else
            {
                SaveData sd = GameManager.Instance.SaveManager.LoadIntoMemory(GameManager.Instance.SaveManager.ActiveSettingsData.lastSaveSlot);
                if (sd.GetBoolVariable("hardmode"))
                {
                    continueButton.GetComponent<BasicButton>().onClick.AddListener(DredgeHardMode.Instance.buttonClickAction);
                }
            }
        }
    }
}
