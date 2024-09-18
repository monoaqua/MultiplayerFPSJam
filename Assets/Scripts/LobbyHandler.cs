using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;

public class LobbyHandler : NetworkBehaviour {
    private GameManager gameManager;

    [SerializeField] private GameObject lobbyPlayerList;
    [SerializeField] private GameObject lobbyPlayerCard;

    public override void OnStartClient() {
        base.OnStartClient();

        gameManager = InstanceFinder.GetInstance<GameManager>();
        
        gameManager.RoomManager.ConnectedPlayers.OnChange += ConnectedPlayersListChanged;
        UpdateLobbyPlayerList();
    }

    public override void OnStopClient() {
        base.OnStopClient();

        gameManager.RoomManager.ConnectedPlayers.OnChange -= ConnectedPlayersListChanged;

        ClearLobbyPlayerList();
    }

    private void ConnectedPlayersListChanged(SyncListOperation op, int index, RoomPlayer oldItem, RoomPlayer newItem, bool asServer) {
        UpdateLobbyPlayerList();
    }

    private void UpdateLobbyPlayerList() {
        ClearLobbyPlayerList();

        for (int i = 0; i < gameManager.RoomManager.ConnectedPlayers.Count; i++) {
            RoomPlayer lobbyPlayer = gameManager.RoomManager.ConnectedPlayers[i];
            GameObject playerCard = Instantiate(lobbyPlayerCard, lobbyPlayerList.transform);
            playerCard.GetComponentInChildren<TMP_Text>().text = lobbyPlayer.prettyName;
        }
    }

    private void ClearLobbyPlayerList() {
        int childCount = lobbyPlayerList.transform.childCount;
        for (int i = 0; i < childCount; i++) {
            Destroy(lobbyPlayerList.transform.GetChild(i).gameObject);
        }
    }
}
