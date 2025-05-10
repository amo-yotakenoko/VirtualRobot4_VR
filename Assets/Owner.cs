using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Owner : MonoBehaviour
{
    // Start is called before the first frame update
    public int id;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        id = (int)GetComponent<NetworkObject>().OwnerClientId;
    }
}
