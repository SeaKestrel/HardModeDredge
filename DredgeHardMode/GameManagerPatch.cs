using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Winch.Core;

namespace DredgeHardMode
{
    [HarmonyPatch(typeof(GameManager))]
    internal class GameManagerPatch
    {
        [HarmonyPatch(nameof(GameManager.BeginGame))]
        [HarmonyPostfix]
        public static void BeginGame()
        {
            WinchCore.Log.Debug("Patched BeginGame()");
            try
            {
                if (DredgeHardMode.Instance.ShouldBeHard) DredgeHardMode.Instance.OnGameStarted();
            } catch (Exception ex)
            {
                WinchCore.Log.Error(ex);
            }
        }
    }
}
