using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Winch.Core;
using Winch.Util;

namespace DredgeHardMode.MainMenu
{
    [HarmonyPatch(typeof(SaveSlotUI))]
    internal class SaveSlotUIModifier : MonoBehaviour
    {

        public static SaveSlotUI saveSlotUI;

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
                    saveSlotUI = __instance;

                    GameObject saveSlot = __instance.gameObject; // Gets the current SaveSlotUI gameObject

                    GameObject startHardButton = Instantiate(saveSlot.transform.GetChild(2)).gameObject; // Gets the LoadOrNewButton gameObject
                    startHardButton.SetActive(false);
                    startHardButton.name = "HardmodeButton";

                    Destroy(startHardButton.transform.GetChild(0).GetComponent<LocalizeStringEvent>());
                    startHardButton.GetComponentInChildren<TextMeshProUGUI>().text = "Hardmode";
                    startHardButton.transform.GetChild(1).GetComponent<Image>().sprite = TextureUtil.GetSprite("hardmode_icon");

                    startHardButton.transform.SetParent(saveSlot.transform, false);
                    startHardButton.transform.position = saveSlot.transform.GetChild(3).position; // Sets the new button to the DeleteButton's position

                    startHardButton.GetComponent<BasicButtonWrapper>().OnClick = saveSlot.transform.GetChild(2).GetComponent<BasicButtonWrapper>().OnClick;
                    startHardButton.GetComponent<BasicButton>().onClick.AddListener(DredgeHardMode.Instance.shouldBeHardAction);

                    startHardButton.SetActive(true);
                    DontDestroyOnLoad(startHardButton);
                }
                catch (Exception ex)
                {
                    WinchCore.Log.Error(ex);
                }
            }
            else if (__instance.saveData.GetBoolVariable("hardmode")) /* Add listener to the Continue button and text*/
            {
                __instance.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text += "\nHardmode";
                __instance.selectSlotButton.button.onClick.AddListener(DredgeHardMode.Instance.shouldBeHardAction);
            }

        }
    }
}
