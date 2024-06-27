using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Winch.Core;

namespace DredgeHardMode.Save
{
    [HarmonyPatch(typeof(SaveManager))]
    internal class SaveManagerPatch
    {
        [HarmonyPatch(nameof(SaveManager.Save))]
        [HarmonyPrefix]
        public static void Save()
        {
            WinchCore.Log.Debug("Patched SaveManager.Save");
            DredgeHardMode.Instance.Config.Save();
        }

        [HarmonyPatch(nameof(SaveManager.Load))]
        [HarmonyPostfix]
        public static void Load()
        {
            WinchCore.Log.Debug("Patched SaveManager.Load");
            DredgeHardMode.Instance.Config.Load();
        }

        [HarmonyPatch(nameof(SaveManager.CreateSaveData))]
        [HarmonyPostfix]
        public static void CreateSaveData()
        {
            WinchCore.Log.Debug("Patched SaveManager.CreateSaveData");
            DredgeHardMode.Instance.Config.Load();
        }
    }
}
