using UnityEngine;
using UnityEngine.Events;
using Winch.Core;
using static UnityEngine.UI.Button;

namespace DredgeHardMode
{
    [AddToMainMenuScene]
    internal class MainMenuModifier : MonoBehaviour
    { 

        static UnityAction action = OnClick;
        public static ButtonClickedEvent buttonClickedEvent;
        public static int slot;

        public void Awake()
        {
            
        }

        public static void OnClick()
        {
            WinchCore.Log.Error("Pressing button");
        }
    }
}
