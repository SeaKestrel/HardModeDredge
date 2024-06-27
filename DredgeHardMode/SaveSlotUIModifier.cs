using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Winch.Config;
using Winch.Core;
using Winch.Util;
using static UnityEngine.UI.Button;

namespace DredgeHardMode
{
    [HarmonyPatch(typeof(SaveSlotUI))]
    internal class SaveSlotUIModifier : MonoBehaviour
    {

        static UnityAction action = OnClick;
        public static ButtonClickedEvent buttonClickedEvent;
        public static int slot;

        [HarmonyPatch(nameof(SaveSlotUI.SetupUI))]
        [HarmonyPostfix]
        public static void SetupUI(ref SaveSlotUI __instance)
        {
            if (!__instance.hasSaveFile)
            {
                GameObject slotList = GameObject.Find("Canvases/MenuCanvas/SaveSlotWindow/Container/Panel/SlotList");

                /*
                 * Add the hardmode button
                 */
                try
                {
                    Transform child = slotList.transform.GetChild(__instance.slotNum); // Gets the save slot
                    GameObject saveSlot = __instance.gameObject; // Gets the current SaveSlotUI gameObject
                    WinchCore.Log.Error(saveSlot.name);

                    GameObject button = Instantiate(saveSlot.transform.GetChild(2)).gameObject; // Gets the LoadOrNewButton gameObject
                    button.SetActive(false);

                    /*WinchCore.Log.Error(button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                    button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Hardmode");
                    WinchCore.Log.Error(button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);*/

                    Destroy(button.transform.GetChild(0).GetComponent<LocalizeStringEvent>());
                    button.GetComponentInChildren<TextMeshProUGUI>().text = "Hardmode";
                    button.transform.GetChild(1).GetComponent<Image>().sprite = TextureUtil.GetSprite("hardmode_icon");


                    button.transform.SetParent(saveSlot.transform, false);
                    button.transform.position = saveSlot.transform.GetChild(3).position; // Sets the new button to the DeleteButton's position

                    BasicButton basicButton = button.GetComponent<BasicButton>();
                    basicButton.onClick.AddListener(action);

                    button.SetActive(true);
                    DontDestroyOnLoad(button);
                }
                catch (Exception ex)
                {
                    WinchCore.Log.Error(ex);
                }
            }
            else if(__instance.saveData.GetBoolVariable("hardmode")) /* Add listener to the Continue button */
            {
                __instance.selectSlotButton.button.onClick.AddListener(action);
            }
            
        }

        /// <summary>
        /// Button handler for the event
        /// </summary>
        public static void OnClick()
        {
            WinchCore.Log.Error("Pressing button");
            DredgeHardMode.Instance.ShouldBeHard = true;
        }
    }
}
