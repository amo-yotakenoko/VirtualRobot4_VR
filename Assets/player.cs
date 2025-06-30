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
using System.Linq;
public class player : Unity.Netcode.NetworkBehaviour
{
    // Start is called before the first frame update
    public static player ownerPlayer;

    public GameObject robotPrefab;

    public GameObject robot;
    public robotController robotController;

    public void setRobot(GameObject r)
    {
        robot = r;
        robotController = r.GetComponent<robotController>();
    }




    public override void OnNetworkSpawn()
    {
        Debug.Log($"OnNetworkSpawn called for GameObject: {gameObject.name}, Instance ID: {GetInstanceID()}, NetworkObjectId: {NetworkObjectId}, IsServer: {IsServer}, IsClient: {IsClient}  ");
        if (IsOwner && robot == null)
        {

            RequestSpawnServerRpc();
        }

        if (NetworkObject.IsLocalPlayer) ownerPlayer = this;
    }

    [ServerRpc]
    void RequestSpawnServerRpc(ServerRpcParams rpcParams = default)
    {
        robot = Instantiate(robotPrefab, transform.position, Quaternion.identity);
        robot.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }


    // Update is called once per frame
    void Update()
    {

    }
    [Rpc(SendTo.Server)]
    public void rescueServerRpc(Vector3 offset)
    {
        foreach (var p in robot.GetComponent<generateRobot>().partspos)
        {
            // p.Key.transform.localPosition = p.Value;
            // p.Key.transform.rotation = Quaternion.identity;
            StartCoroutine(MovePartToPosition(p.Key, p.Value + offset));
        }
    }

    private IEnumerator MovePartToPosition(GameObject part, Vector3 targetPosition)
    {

        var enableColliders = part.GetComponentsInChildren<Collider>().Where(x => x.enabled).ToArray();
        foreach (var col in enableColliders) col.enabled = false;




        float moveDuration = 1.0f;
        Vector3 startPosition = part.transform.localPosition;
        Quaternion startRotation = part.transform.rotation;
        Quaternion targetRotation = Quaternion.identity;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            part.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            part.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
            print(enableColliders.Count());
        }

        // 最後にターゲット位置と回転にスナップする
        part.transform.localPosition = targetPosition;
        part.transform.rotation = targetRotation;


        foreach (var col in enableColliders)
        {
            col.enabled = true;

        }
        var enableRigidbody = part.GetComponent<Rigidbody>();
        if (enableRigidbody != null && !enableRigidbody.isKinematic)
        {

            enableRigidbody.velocity = Vector3.zero;
            enableRigidbody.angularVelocity = Vector3.zero;
            enableRigidbody.Sleep();
        }
    }

    public void robotChange()
    {
        DestroyRobotServerRpc();
        RequestSpawnServerRpc();
    }

    [ServerRpc]
    void DestroyRobotServerRpc(ServerRpcParams rpcParams = default)
    {
        robot.GetComponent<NetworkObject>().Despawn();
        Destroy(robot.gameObject);
    }



}
