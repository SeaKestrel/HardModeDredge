using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using Winch.Core;

namespace DredgeHardMode
{
    [HarmonyPatch(typeof(SaveSlotUI))]
    internal class SaveSlotUIModifier : MonoBehaviour
    {

        static UnityAction action;

        [HarmonyPatch(nameof(SaveSlotUI.SetupUI))]
        [HarmonyPostfix]
        public static void SetupUI(ref SaveSlotUI __instance)
        {
            GameObject slotList = GameObject.Find("Canvases/MenuCanvas/SaveSlotWindow/Container/Panel/SlotList");

            try
            {
                Transform child = slotList.transform.GetChild(__instance.slotNum);
                WinchCore.Log.Error(child.name);

                GameObject button = Instantiate(child.GetChild(2)).gameObject;
                button.SetActive(true);
                button.transform.parent = child;
                button.transform.position = child.GetChild(3).position;
                button.layer = 999;

                BasicButton basicButton = button.GetComponent<BasicButton>();
                action += OnClick; 
                basicButton.onClick.AddListener(action);

                DontDestroyOnLoad(button);
            }
            catch (Exception ex)
            {
                WinchCore.Log.Error(ex);
            }
        }

        public static void OnClick()
        {
            DredgeHardMode.Instance.ShouldBeHard = true;
        }
    }
}
