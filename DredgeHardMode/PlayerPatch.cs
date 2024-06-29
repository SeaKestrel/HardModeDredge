using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Steamworks;

namespace DredgeHardMode
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        [HarmonyPatch(nameof(Player.Die), new Type[] {})]
        [HarmonyPostfix]
        public static void DiePatch()
        {
            DredgeHardMode.Instance.died = true;
        }
    }
}
