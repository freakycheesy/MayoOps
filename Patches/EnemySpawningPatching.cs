using HarmonyLib;
using UnityEngine;
using Riptide;
using MayoOps.Messages;

namespace MayoOps.Patches {
    [HarmonyPatch(typeof(EnemySpawn), "spawnEnemy")]
    public static class EnemySpawningPatching {
        [HarmonyPrefix]
        public static bool Prefix(EnemySpawn __instance) {
            EnemySpawningMessage.MAIN = __instance;
            if (!Plugin.client.IsConnected && !Plugin.server.IsRunning)
                return true;
            if (!Plugin.server.IsRunning)
                return false;
            int idx = Random.Range(0, 4);
            var message = Message.Create(MessageSendMode.Reliable, MessageIds.SpawnEnemy);
            message.Add(idx);
            Plugin.server.SendToAll(message);
            return false;
        } 
    }
}
