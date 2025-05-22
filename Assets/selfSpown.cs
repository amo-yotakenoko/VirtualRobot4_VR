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
        GetComponent<Unity.Netcode.NetworkObject>().Spawn();
    }
}
