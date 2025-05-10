using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
public class keyController : Unity.Netcode.NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public robotController robotController;
    void Update()
    {
        if (this.IsOwner)
        {


            // 前進
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                robotController.setvalue("A", "power", 200);
                robotController.setvalue("B", "power", 200);
            }
            // 後退
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                robotController.setvalue("A", "power", -200);
                robotController.setvalue("B", "power", -200);
            }
            // 左に旋回
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                robotController.setvalue("A", "power", 200);
                robotController.setvalue("B", "power", -200);
            }
            // 右に旋回
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                robotController.setvalue("A", "power", -200);
                robotController.setvalue("B", "power", 200);
            }

            // 何も押されていないときは停止
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                robotController.setvalue("A", "power", 0);
                robotController.setvalue("B", "power", 0);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                robotController.setvalue("C", "power", 100);

            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                robotController.setvalue("C", "power", -100);

            }
            if (Input.GetKeyUp(KeyCode.Z))
            {
                robotController.setvalue("C", "power", 0);

            }
            if (Input.GetKeyUp(KeyCode.X))
            {
                robotController.setvalue("C", "power", 0);

            }
        }
    }
    // [ServerRpc]
    // void  robotController.setvalue(string name, string property, float value)
    // {
    //     robotController.setvalue(name, property, value);
    // }

}