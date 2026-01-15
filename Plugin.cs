using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MayoOps.Messages;
using Riptide;
using Riptide.Transports.Steam;
using Riptide.Transports.Udp;
using Riptide.Utils;
using Steamworks;
using System;
using System.Threading.Tasks;
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
            UseSteamNetworking = true;
            SteamAPI.Init();

            Task.Factory.StartNew(RiptideThread);
        }

        private void RiptideThread() {
            while (Application.isPlaying) {
                if (server.IsRunning)
                    server.Update();
                client.Update();
                return;
            }
            server.Stop();
            client.Disconnect();
            LobbyManager.LeaveLobby();
            server = null;
            client = null;
        }

        void Start() => LobbyManager.Start();
        internal string address;
        private static bool steamNetworking = true;
        public static bool UseSteamNetworking {
            get => steamNetworking; set {
                steamNetworking = value;
                if (steamNetworking) {
                    var steamServer = new SteamServer();
                    server = new(steamServer);
                    client = new(new Riptide.Transports.Steam.SteamClient(steamServer));
                }
                else {
                    server = new(new UdpServer());
                    client = new(new UdpClient());
                }
            }
        }
        internal void OnGUI() {
            if (GUILayout.Button("Create Server")) {
                LobbyManager.CreateLobby();
                if(UseSteamNetworking)
                    server.Start(0, 16);
                else {
                    server.Start(7777, 16);
                }
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
            UseSteamNetworking = GUILayout.Toggle(UseSteamNetworking, "USE STEAM!?!?!");
            GUILayout.Box($"Hosting:{server.IsRunning}\nConnected: {client.IsConnected}\n");
        }

        private void Shutdown() {
            server.Stop();
            client.Disconnect();
            LobbyManager.LeaveLobby();
        }

        public void JoinLocalServer() {
            try {
                if (UseSteamNetworking)
                    client.Connect("127.0.0.1");
                else
                    client.Connect("localhost:7777");
            }
            catch {

            }
        }

        internal void Update() {
            PlayerMessages.Update();
            SteamAPI.RunCallbacks();
        }
    }
}
