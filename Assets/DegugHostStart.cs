using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class DegugHostStart : MonoBehaviour
{

    void Update()
    {
        if (NetworkManager.Singleton == null)
            return;

        bool isConnected = NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer;
        this.gameObject.SetActive(!isConnected); // 接続中は非表示、未接続なら表示
    }
}
