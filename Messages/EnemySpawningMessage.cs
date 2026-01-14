using MayoOps.Patches;
using Riptide;
using UnityEngine;

namespace MayoOps.Messages {
    public static class EnemySpawningMessage {
        public static EnemySpawn MAIN;

        [MessageHandler((ushort)MessageIds.SpawnEnemy)]
        public static void SpawnEnemy_Client(Message message) {
            SpawnEnemy(message.GetInt());
        }

        public static void SpawnEnemy(int idx) {
            switch (idx) {
                case 0:
                    Object.Instantiate(MAIN.ketchupPrefab, MAIN.transform.position + Vector3.up, MAIN.transform.rotation);
                    break;
                case 1:
                    Object.Instantiate(MAIN.mustardPrefab, MAIN.transform.position + Vector3.up, MAIN.transform.rotation);
                    break;
                case 2:
                    Object.Instantiate(MAIN.relishPrefab, MAIN.transform.position + Vector3.up, MAIN.transform.rotation);
                    break;
                case 3:
                    Object.Instantiate(MAIN.bbqPrefab, MAIN.transform.position + Vector3.up, MAIN.transform.rotation);
                    break;
            }
        }
    }
}
