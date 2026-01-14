using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MayoOps.Messages;
using Riptide;
using Riptide.Transports.Steam;
using Riptide.Transports.Udp;
using Riptide.Utils;
using System;
using UnityEngine;

namespace MayoOps
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        public static Harmony harmony;
        public static Server server = new();
        public static Client client = new();
        internal void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            harmony = new(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            RiptideLogger.Initialize(Logger.LogInfo, Logger.LogInfo, Logger.LogWarning, Logger.LogError, false);
            SteamNetworking = true;
        }

        void Start() => LobbyManager.Start();
        internal string address;
        private static bool steamNetworking = true;
        public static bool SteamNetworking {
            get => steamNetworking; set {
                steamNetworking = value;
                if (steamNetworking) {
                    server.ChangeTransport(new SteamServer());
                    client.ChangeTransport(new SteamClient());
                }
                else {
                    server.ChangeTransport(new UdpServer());
                    client.ChangeTransport(new UdpClient());
                }
            }
        }
        internal void OnGUI() {
            if (GUILayout.Button("Create Server")) {
                LobbyManager.CreateLobby();
                server.Start(7777, 16);
                JoinLocalServer();
            }
            address = GUILayout.TextField(address);
            if (GUILayout.Button("Connect to Server")) {
                if (!client.Connect(address))
                    LobbyManager.JoinLobby(ulong.Parse(address));
            }
            if(GUILayout.Button("Join Local Server")) JoinLocalServer();
            if (GUILayout.Button("Shutdown"))
                Shutdown();
            GUILayout.Box($"Hosting:{server.IsRunning}\nConnected: {client.IsConnected}\n");
        }

        private void Shutdown() {
            server.Stop();
            client.Disconnect();
            LobbyManager.LeaveLobby();
        }

        public void JoinLocalServer() {
            if (!client.Connect("127.0.0.1:7777")) {
                if (!client.Connect("127.0.0.1"))
                    if (!client.Connect("localhost:7777"))
                        client.Connect("localhost");
            }
        }

        internal void Update() {
            PlayerMessages.Update();
            server.Update();
            client.Update();
        }
    }
}
