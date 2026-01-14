using HarmonyLib;
using MayoOps.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MayoOps.Patches {
    [HarmonyPatch(typeof(playerController), "Start")]
    public static class PlayerPatching {
        [HarmonyPostfix]
        public static void StartPostfix(playerController __instance) {
            PlayerMessages.MAIN = __instance;
        }
    }
}
