using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfSpown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Unity.Netcode.NetworkObject>().Spawn();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
