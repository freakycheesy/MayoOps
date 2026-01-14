using MayoOps.Runtime;
using Riptide;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MayoOps.Messages {
    static class PlayerMessages {
        public static playerController MAIN;

        public static void Update() {
            if (!Plugin.client.IsConnected)
                return;
            SendData();
            foreach (var player in players.Keys) {
                players[player].OnUpdate();
            }
        }

        private static void SendData() {
            var message = Message.Create(MessageSendMode.Unreliable, MessageIds.PlayerState);
            message.Add(MAIN.transform.position);
            message.Add(MAIN.transform.rotation);
            Plugin.client.Send(message);
        }

        public static Dictionary<ushort, PlayerRef> players = new();

        [MessageHandler((ushort)MessageIds.PlayerState)]
        public static void HandlePlayer_Server(ushort sender, Message received) {
            var message = Message.Create(MessageSendMode.Unreliable, MessageIds.PlayerState);
            message.Add(sender);
            message.AddMessage(received);
            Plugin.server.SendToAll(message, sender);
        }

        [MessageHandler((ushort)MessageIds.PlayerState)]
        public static void HandlePlayer_Client(Message received) {
            var id = received.GetUShort();
            var pos = received.GetVector3();
            var rot = received.GetQuaternion();
            if (!players.ContainsKey(id) || players[id].Instance == null) {
                CreatePlayer(id);
            }

            players[id].UpdateData(pos, rot);
        }

        private static void CreatePlayer(ushort id) {
            var playerInstance = GameObject.Instantiate(MAIN.transform);
            var playerRef = new PlayerRef(playerInstance);
            players.Add(id, playerRef);
        }
    }
}
