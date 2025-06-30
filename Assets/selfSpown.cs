using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class selfSpown : MonoBehaviour
{
    // Start is called before the first frame update


    void Start()
    {
        StartCoroutine(WaitAndSpawn());
    }

    private IEnumerator WaitAndSpawn()
    {
        // Wait until the NetworkManager is listening
        yield return new WaitUntil(() => NetworkManager.Singleton.IsListening);
        yield return new WaitForSeconds(0.1f); // 少し待つことで、NetworkManagerが完全に初期化されるのを確実にする

        // すでにスポーン済みでなければスポーンする
        if (!GetComponent<NetworkObject>().IsSpawned)
        {

            GetComponent<Unity.Netcode.NetworkObject>().Spawn();

        }

        Destroy(this);
    }
}
