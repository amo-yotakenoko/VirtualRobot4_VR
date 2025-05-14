using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Mathematics;

using UnityEngine.SceneManagement;
using Unity.Netcode;
public class rescue : Unity.Netcode.NetworkBehaviour
{
    // Start is called before the first frame update

    //シングルトンパターン(?)
    public static rescue Instance;
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void rescueStart()
    {
        rescueStart(new Vector3(0, 0, 0));
    }
    public void rescueStart(Vector3 offset)
    {


        player.ownerPlayer.rescueServerRpc(offset);
    }

}
