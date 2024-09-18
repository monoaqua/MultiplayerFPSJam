using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using MyBox;

public class MenuGUI : MonoBehaviour {
    private NetworkManager networkManager;
    private Transport transport;
    private SceneManager sceneManager;

    private enum Menu {
        Connection,
        Lobby
    }
    private Menu currentMenu;

    [Foldout("Connection GUI", true)]
    [SerializeField] private GameObject connectionGUI;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    [Foldout("Lobby GUI", true)]
    [SerializeField] private GameObject lobbyGUI;
    [SerializeField] private LobbyHandler lobbyHandler;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button startButton;

    private void Awake() {
        networkManager = InstanceFinder.NetworkManager;
        sceneManager = networkManager.SceneManager;
        transport = networkManager.TransportManager.GetTransport<Tugboat>();
    }

    private void Start() {
        SetMenu(Menu.Connection);
    }

    private void JoinLobby(bool asHost) {
        // Update connection data
        if (addressInput.text != "") {
            transport.SetClientAddress(addressInput.text);
        } else {
            transport.SetClientAddress(((TextMeshProUGUI) addressInput.placeholder).text);
        }

        if (portInput.text != "") {
            if (ushort.TryParse(portInput.text, out ushort ushortPort)) {
                transport.SetPort(ushortPort);
            }
        } else {
            if (ushort.TryParse(((TextMeshProUGUI) portInput.placeholder).text, out ushort ushortPort)) {
                transport.SetPort(ushortPort);
            }
        }

        // Start connection
        if (asHost) {
            transport.StartConnection(true);
        }

        transport.StartConnection(false);

        SetMenu(Menu.Lobby);
    }

    private void HostButtonOnClick() {
        JoinLobby(true);
    }

    private void JoinButtonOnClick() {
        JoinLobby(false);
    }

    private void LeaveButtonOnClick() {
        transport.StopConnection(InstanceFinder.IsServer);
        
        SetMenu(Menu.Connection);
    }

    private void StartButtonOnClick() {
        InstanceFinder.GetInstance<GameManager>().RoomManager.StartGame();
    }

    private void SetMenu(Menu menu) {
        if (menu == Menu.Connection) {
            lobbyGUI.SetActive(false);
            connectionGUI.SetActive(true);
        } else if (menu == Menu.Lobby) {
            connectionGUI.SetActive(false);
            lobbyGUI.SetActive(true);
        }

        currentMenu = menu;
    }

    private void OnEnable() {
        hostButton.onClick.AddListener(HostButtonOnClick);
        joinButton.onClick.AddListener(JoinButtonOnClick);
        leaveButton.onClick.AddListener(LeaveButtonOnClick);
        startButton.onClick.AddListener(StartButtonOnClick);
    }

    private void OnDisable() {
        hostButton.onClick.RemoveListener(HostButtonOnClick);
        joinButton.onClick.RemoveListener(JoinButtonOnClick);
        leaveButton.onClick.RemoveListener(LeaveButtonOnClick);
        startButton.onClick.RemoveListener(StartButtonOnClick);
    }
}
