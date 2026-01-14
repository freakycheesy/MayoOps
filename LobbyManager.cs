using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MayoOps {
    public static class LobbyManager {

        public static Callback<LobbyCreated_t> lobbyCreated;
        public static Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        public static Callback<LobbyEnter_t> lobbyEnter;

        public const string HostAddressKey = "HostAddress";
        public static CSteamID lobbyId;

        public static void Start() {
            if (!SteamManager.Initialized) {
                Debug.LogError("Steam is not initialized!");
                return;
            }

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        }

        public static void CreateLobby() {
            if (!Plugin.SteamNetworking)
                return;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 5);
        }

        public static void OnLobbyCreated(LobbyCreated_t callback) {
            if (!Plugin.SteamNetworking)
                return;
            if (callback.m_eResult != EResult.k_EResultOK) {
                return;
            }

            lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        }

        public static void JoinLobby(ulong lobbyId) {
            if (!Plugin.SteamNetworking)
                return;
            SteamMatchmaking.JoinLobby(new CSteamID(lobbyId));
        }

        public static void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) {
            if (!Plugin.SteamNetworking)
                return;
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        public static void OnLobbyEnter(LobbyEnter_t callback) {
            if (!Plugin.SteamNetworking)
                return;
            if (Plugin.server.IsRunning)
                return;

            lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
            CSteamID hostId = SteamMatchmaking.GetLobbyOwner(lobbyId);

            Plugin.client.Connect(hostId.ToString());
        }

        public static void LeaveLobby() {
            if (!Plugin.SteamNetworking)
                return;
            SteamMatchmaking.LeaveLobby(lobbyId);
        }
    }
}
