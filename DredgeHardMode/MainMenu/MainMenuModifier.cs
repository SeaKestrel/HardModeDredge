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
            GameObject continueButton = GameObject.Find("Canvases/MenuCanvas/ButtonContainer/").transform.GetChild(0).gameObject; // Gets the original continue button
            ContinueOrNewButton continueOrNewButton = continueButton.GetComponent<ContinueOrNewButton>(); // Gets the component
            if (continueOrNewButton.currentMode == ContinueOrNewButton.StartButtonMode.NEW)
            {
                continueButton.GetComponent<BasicButton>().onClick.AddListener(DredgeHardMode.Instance.shouldBeHardAction); // Adding listener anyway to **create** a hardmode save
            }
            else
            {
                SaveData sd = GameManager.Instance.SaveManager.LoadIntoMemory(GameManager.Instance.SaveManager.ActiveSettingsData.lastSaveSlot);
                if (sd.GetBoolVariable("hardmode"))
                {
                    continueButton.GetComponent<BasicButton>().onClick.AddListener(DredgeHardMode.Instance.shouldBeHardAction); // If the save is in hardmode, adding the listener
                }
            }
        }
    }
}
