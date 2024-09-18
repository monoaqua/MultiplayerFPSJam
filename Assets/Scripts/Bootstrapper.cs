using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Bootstrapper : NetworkBehaviour {
    [SerializeField] private GameObject gameManagerPrefab;
    private GameObject gameManager;

    public override void OnStartServer() {
        base.OnStartServer();

        gameManager = Instantiate(gameManagerPrefab);
        NetworkObject.Spawn(gameManager);
    }
}
