using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using TMPro;
// using UnityEditor.PackageManager;

// [GenerateSerializationForType(typeof(string))]
public class PartSync : NetworkBehaviour
{
    public NetworkVariable<Unity.Collections.FixedString64Bytes> partname = new NetworkVariable<Unity.Collections.FixedString64Bytes>("");

    void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // サーバー側で直接変更
            partname.Value = this.name;
        }
    }

    void Update()
    {
        // print(NetworkManager.Singleton.IsServer);

        if (!NetworkManager.Singleton.IsServer)
        {
            // print("Client");
            this.name = $"Sync_{partname.Value}";
            // Debug.Log($"Part name on client: {partname.Value}");
        }
    }
}