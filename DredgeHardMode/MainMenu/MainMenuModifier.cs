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
            continueButton.GetComponent<BasicButton>().onClick.AddListener(DredgeHardMode.Instance.buttonClickAction);
        }
    }
}
