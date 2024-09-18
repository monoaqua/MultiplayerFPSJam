using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class GameManager : NetworkBehaviour {
    public RoomManager RoomManager { get; private set; }
    public SpawnHandler SpawnHandler { get; private set; }

    [SerializeField] private GameSettings gameSettings;

    private void Awake() {
        InstanceFinder.RegisterInstance<GameManager>(this);

        if (TryGetComponent<RoomManager>(out RoomManager roomManager)) {
            RoomManager = roomManager;
        } else {
            RoomManager = gameObject.AddComponent<RoomManager>();
        }

        if (TryGetComponent<SpawnHandler>(out SpawnHandler spawnHandler)) {
            SpawnHandler = spawnHandler;
        } else {
            SpawnHandler = gameObject.AddComponent<SpawnHandler>();
        }
    }
}
