using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using MyBox;

public class RoomManager : NetworkBehaviour {
    [SyncObject]
    public readonly SyncList<RoomPlayer> ConnectedPlayers = new SyncList<RoomPlayer>();

    [SerializeField] private SceneReference testScene;

    public override void OnStartClient() {
        base.OnStartClient();

        if (IsServer) {
            // DEDSER: Change leadership detection
            ConnectedPlayers.Clear();
            foreach (KeyValuePair<int, NetworkConnection> client in NetworkManager.ServerManager.Clients) {
                RoomPlayer roomPlayer = new RoomPlayer(client.Key);
                roomPlayer.isLeader = true;
                ConnectedPlayers.Add(roomPlayer);
            }

            NetworkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
            // NetworkManager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
        }
    }

    public override void OnStopClient() {
        base.OnStopClient();

        if (IsServer) {
            NetworkManager.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
            // NetworkManager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;

            ConnectedPlayers.Clear();
        }
    }

    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args) {
        if (args.ConnectionState == RemoteConnectionState.Started) {
            ConnectedPlayers.Add(new RoomPlayer(conn.ClientId));
        } else if (args.ConnectionState == RemoteConnectionState.Stopped) {
            ConnectedPlayers.RemoveAt(ConnectedPlayers.FindIndex((RoomPlayer roomPlayer) => roomPlayer.clientId == conn.ClientId));
        }
    }

    public RoomPlayer GetConnectedPlayer(int clientId) {
        return ConnectedPlayers.Find((RoomPlayer roomPlayer) => roomPlayer.clientId == clientId);
    }

    public RoomPlayer GetCurrentPlayer() {
        return ConnectedPlayers.ToList().Find((RoomPlayer roomPlayer) => roomPlayer.clientId == NetworkManager.ClientManager.Connection.ClientId);
    }

    public void StartGame() {
        StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc(NetworkConnection conn = null) {
        if (GetConnectedPlayer(conn.ClientId).isLeader) {
            SceneLoadData sld = new SceneLoadData(testScene.SceneName);
            sld.ReplaceScenes = ReplaceOption.All;
            NetworkManager.SceneManager.LoadGlobalScenes(sld);
        } else {
            Debug.LogWarning("Only the owner can start the game!");
        }
    }

    private void OnClientLoadedStartScenes(NetworkConnection conn, bool asServer) {
        Debug.Log($"{conn.ClientId} ... {asServer}");
    }
}
