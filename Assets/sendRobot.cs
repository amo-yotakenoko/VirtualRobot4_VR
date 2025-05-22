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
using System.Runtime.InteropServices.WindowsRuntime;

public class sendRobot : Unity.Netcode.NetworkBehaviour
{
    // Start is called before the first frame update

    public string filePath;
    // [TextArea(3, 100)]
    public NetworkVariable<Unity.Collections.FixedString64Bytes> robotname = new NetworkVariable<Unity.Collections.FixedString64Bytes>("");
    public string data64;

    public bool datacomplete;

    public NetworkVariable<int> fileHash = new NetworkVariable<int>(0,
    NetworkVariableReadPermission.Everyone, // 全員が読み取れる
    NetworkVariableWritePermission.Owner     // オーナーのみ書き込める
    );

    // public TMP_InputField passField;
    public override void OnNetworkSpawn()
    {

        fileHash.OnValueChanged += OnFileHashUpdate;
        OnFileHashUpdate(0, fileHash.Value);
        if (NetworkManager.Singleton.IsServer)
        {
            // robotname.Value = $"{Guid.NewGuid()}";
            if (this.tag == "Player") robotname.Value = $"player_{OwnerClientId}_{Guid.NewGuid()}";
            else robotname.Value = $"{Guid.NewGuid()}";
        }
        // {
        //     var robot = NetworkManager.Singleton.SceneManager.LoadScene("robot", LoadSceneMode.Additive);
        //     SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetSceneByName("robot"));
        // }
        textMesh.transform.parent = transform.root;

        NetworkManager networkManager = NetworkManager.Singleton;
        // transform.position = new Vector3(0, 0, this.OwnerClientId * 10);
        gameObject.name += NetworkObject.IsLocalPlayer ? "IsLocalPlayer" : "";

        if (NetworkObject.IsOwner)
        {

            ownerLoad();


        }

        // if (NetworkObject.IsOwner)
        // {
        //     print("id" + NetworkManager.Singleton.LocalClientId);
        //     // filePath = GameObject.Find("glbPassInput").GetComponent<TMP_InputField>().text.Replace("\"", "");
        //     if (filePath == "")
        //         filePath = robotSelect.glbFullPath;
        //     print("filePath" + filePath);
        //     // filePath = @$"C:/Users/taken/projects/VirtualRobot4/robot1/test1.glb";
        //     // filePath = @$"./test1.glb";

        //     data64 = FileToBase64(filePath);

        //     if (this.IsServer)
        //     {

        //         fileHash.Value = FNV1a(data64);
        //         datacomplete = true;
        //     }
        //     else
        //         StartCoroutine(sendToServer());


        // }
        // else if (NetworkManager.Singleton.IsServer && filePath != "")
        // {
        //     // print("field");
        //     data64 = FileToBase64(filePath);

        //     // this.gameObject.MoveScenes(robot);
        //     datacomplete = true;


        // }





        // if ((int)this.OwnerClientId == (int)NetworkManager.Singleton.LocalClientId)
        // {
        // }


        // if (!this.IsOwner && networkManager.IsClient)
        // {
        // print("クライアント");
        // if (!NetworkManager.Singleton.IsServer && data64.Length == 0)
        // {
        //     this.name += "データほしい";
        //     print("データほしい1" + NetworkManager.Singleton);
        //     WantDataServerRpc(NetworkManager.Singleton.LocalClientId, this.OwnerClientId);
        // }
        // }
    }


    //普通に読み込む
    void ownerLoad()
    {
        // filePath = GameObject.Find("glbPassInput").GetComponent<TMP_InputField>().text.Replace("\"", "");
        if (filePath == "")
            filePath = robotSelect.glbFullPath;
        print("filePath" + filePath);
        // filePath = @$"C:/Users/taken/projects/VirtualRobot4/robot1/test1.glb";
        // filePath = @$"./test1.glb";

        data64 = FileToBase64(filePath);
        datacomplete = true;
        fileHash.Value = FNV1a(data64);
        // Destroy(textMesh.gameObject);
        StartCoroutine(generate());
    }


    void OnFileHashUpdate(int previousValue, int newValue)
    {
        if (newValue == 0) return;
        if (datacomplete) return;


        print("ハッシュ更新");
        this.name = $"ハッシュ更新{newValue}";
        // if (this.IsServer)
        // {

        //     fileHash.Value = FNV1a(data64);
        //     datacomplete = true;
        // }


        string cacheFilePath = getcacheRobotFile();
        if (cacheFilePath != null)
        {
            print("キャッシュを発見" + cacheFilePath);
            this.name = $"キャッシュから更新";
            data64 = FileToBase64(cacheFilePath);
            datacomplete = true;
            // Destroy(textMesh.gameObject);
            StartCoroutine(generate());
            return;
        }

        if (this.IsClient && !this.IsServer)
        {
            if (NetworkObject.IsOwner)
            {
                this.name = $"サーバーに送信";
                StartCoroutine(sendToServer());
            }
            else
            {
                this.name = $"サーバーに要求";
                WantDataServerRpc(NetworkManager.Singleton.LocalClientId, this.OwnerClientId);
            }
        }
    }


    public int sendTextSize;
    private IEnumerator sendToServer()
    {

        int length = data64.Length;
        // string text = "文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト";
        for (int i = 0; i < data64.Length; i += sendTextSize)
        {
            // 10文字ずつ表示する
            // Debug.Log(text.Substring(i, Mathf.Min(10, text.Length - i)));
            string t = data64.Substring(i, Mathf.Min(sendTextSize, data64.Length - i));
            ReceiveDataServerRpc(t, length);
            textMesh.text = $"{i}/{length}\n{(int)(((float)i / length) * 100)}%";
            yield return null;
        }
        // Destroy(textMesh.gameObject);
        print("送信終わり");
        datacomplete = true;
        // StartCoroutine(generate(File.ReadAllBytes(filePath)))
    }
    [ServerRpc]
    private void ReceiveDataServerRpc(string data, int length)
    {
        if (datacomplete) return;
        // print(data);
        data64 += data;
        textMesh.text = $"{data64.Length}/{length}\n{(int)(((float)data64.Length / length) * 100)}%";
        if (data64.Length == length)
        {
            datacomplete = true;
            cacheRobotFile();
            // print(data64);
            // GetComponent<generateRobot>().isServer = true;
            StartCoroutine(generate());
        }
    }

    [ServerRpc]
    public void destroyRobotServerRpc()
    {
        foreach (var part in GetComponent<generateRobot>().parts)
        {
            part.GetComponent<NetworkObject>().Despawn();
            Destroy(part);
        }
        // Destroy(this.gameObject);
    }

    // [ServerRpc]
    // public void reloadRobotServerRpc()
    // {
    //     destroyRobotServerRpc();

    // }




    [Rpc(SendTo.Server)]
    private void WantDataServerRpc(ulong ClientId, ulong robotId)
    {

        StartCoroutine(SendToClient(ClientId, robotId));
        this.name += "データおくる";
    }

    bool SendToClientComplete = false;
    private IEnumerator SendToClient(ulong ClientId, ulong robotId)
    {
        while (!datacomplete) yield return null;
        print("clientに送る");
        yield return null;
        // string text = FileToBase64(filePath);
        int length = data64.Length;
        print(ClientId + "に" + this.OwnerClientId + "を送る" + "長さ" + data64.Length);
        // string text = "文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト文字列のテスト";
        for (int i = 0; i < data64.Length; i += sendTextSize)
        {
            // 10文字ずつ表示する
            // Debug.Log(text.Substring(i, Mathf.Min(10, text.Length - i)));
            string t = data64.Substring(i, Mathf.Min(sendTextSize, data64.Length - i));
            ReceiveDataClientRpc(t, length, robotId: this.OwnerClientId, rpcParams: RpcTarget.Single(ClientId, RpcTargetUse.Temp));
            print(t + "を送った");
            // textMesh.text = $"{i}/{length}\n{(int)(((float)i / length) * 100)}%";
            yield return null;
        }
        SendToClientComplete = true;
        // Destroy(textMesh.gameObject);

        // StartCoroutine(generate(File.ReadAllBytes(filePath)))
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void ReceiveDataClientRpc(string data, int length, ulong robotId, RpcParams rpcParams = default)
    {
        if (datacomplete) return;
        // print(data);
        print("robotId:" + robotId + "を受け取った" + "長さ" + data);
        if (robotId == this.OwnerClientId)
        {

            data64 += data;
            textMesh.text = $"{data64.Length}/{length}\n{(int)(((float)data64.Length / length) * 100)}%";
            if (data64.Length == length)
            {
                // print(data64);
                datacomplete = true;
                cacheRobotFile();
                StartCoroutine(generate());
            }
        }
    }











    // IEnumerator generateRobot()
    // {
    //     generateRobot generateRobot = GetComponent<generateRobot>();
    //     byte[] bytes = Convert.FromBase64String(data64);
    //     // generateRobot.StartCoroutine(generateRobot.generate(bytes));

    //     yield return generateRobot.generate(bytes);
    //     yield return null;
    //     print("生成終了");
    // }
    // Update is called once per frame
    public TextMeshPro textMesh;
    bool generated = false;
    void Update()
    {
        // this.name = $"{robotname.Value}";
        // // textMesh.text = data64.Length.ToString() + ":" + datacomplete.ToString() + "\n" + this.OwnerClientId + ((int)this.OwnerClientId == (int)NetworkManager.Singleton.LocalClientId ? "\nme" : "");
        // if (generated == false && datacomplete == true)
        // {
        //     print("生成準備OK");
        //     generated = true;
        //     StartCoroutine(generate());
        //     Destroy(textMesh.gameObject);
        //     netWorkUI.pause = false;

        //     cacheRobotFile();
        // }
    }

    void cacheRobotFile()
    {
        byte[] bytes = Convert.FromBase64String(data64);

        string cacheFolderPath = Path.Combine(Directory.GetCurrentDirectory(), ".robotCache");

        // フォルダが存在しない場合は作成
        if (!Directory.Exists(cacheFolderPath))
        {
            Directory.CreateDirectory(cacheFolderPath);
        }

        // ファイル名（例：robot_{robotname}.glb）を設定
        string fileName = $"robot_{fileHash.Value}.glb";
        string fullPath = Path.Combine(cacheFolderPath, fileName);

        // バイト配列をファイルに保存
        File.WriteAllBytes(fullPath, bytes);

    }

    string getcacheRobotFile()
    {
        string cacheFolderPath = Path.Combine(Directory.GetCurrentDirectory(), ".robotCache");

        // フォルダが存在しない場合は作成
        if (!Directory.Exists(cacheFolderPath))
        {
            Directory.CreateDirectory(cacheFolderPath);
        }


        string fileName = $"robot_{fileHash.Value}.glb";
        string fullPath = Path.Combine(cacheFolderPath, fileName);


        if (File.Exists(fullPath))
        {
            return fullPath;
        }
        return null;

    }


    public GameObject NetCodePart;
    IEnumerator generate()
    {

        generateRobot generateRobot = GetComponent<generateRobot>();
        byte[] bytes = Convert.FromBase64String(data64);
        // generateRobot.StartCoroutine(generateRobot.generate(bytes));
        if (this.IsServer)
        {

            // generateRobot.isServer = true;
            generateRobot.partprefab = NetCodePart;
            yield return generateRobot.generate(bytes, isServer: this.IsServer, isOwner: this.IsOwner, name = $"{robotname.Value}");
            foreach (var part in generateRobot.parts)
            {

                // Spawningしたオブジェクトを全クライアントに送信する
                part.GetComponent<NetworkObject>().Spawn();
                part.GetComponent<NetworkObject>().ChangeOwnership(GetComponent<NetworkObject>().OwnerClientId);
            }
        }
        else
        {
            // while (!SendToClientComplete) yield return null;
            yield return new WaitForSeconds(5f);
            yield return generateRobot.generate(bytes, isServer: this.IsServer, isOwner: this.IsOwner, name = $"{robotname.Value}");

            var localNetworkObject = GetComponent<NetworkObject>(); // ローカルのNetworkObjectを取得
            foreach (GameObject p in generateRobot.parts)
            {
                GameObject networkParts = FindObjectsOfType<PartSync>().Where(x => p.name == x.partname.Value).Select(x => x.gameObject).FirstOrDefault();
                if (networkParts != null)
                {

                    p.transform.SetParent(networkParts.transform, worldPositionStays: true);
                    p.transform.localPosition = Vector3.zero;
                    p.transform.localRotation = Quaternion.identity;

                    networkParts.GetComponent<MeshRenderer>().enabled = false;
                    p.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            // yield return new WaitForSeconds(5f);
            // int i = 0;
            // this.name = this.name + this.OwnerClientId.ToString();
            // List<Unity.Netcode.NetworkObject> networkObjects = new List<Unity.Netcode.NetworkObject>();
            // print("networkObjects" + networkObjects.ToList().Count + "generateRobot.parts.Count" + generateRobot.parts.Count);
            // //TODO:これもっとユニークな識別子に
            // while (networkObjects.ToList().Count != generateRobot.parts.Count)
            // {
            //     networkObjects = GameObject.FindGameObjectsWithTag("parts")
            // .Select(x => x.GetComponent<NetworkObject>())
            // .Where(networkObject => networkObject != null && networkObject.OwnerClientId == this.OwnerClientId).ToList();
            //     print("networkObjects" + networkObjects.ToList().Count + "generateRobot.parts.Count" + generateRobot.parts.Count);
            //     yield return null;
            // }
            // yield return new WaitForSeconds(1);
            // foreach (NetworkObject networkObject in networkObjects)
            // {

            //     generateRobot.parts[i].transform.SetParent(networkObject.transform, worldPositionStays: true);
            //     generateRobot.parts[i].transform.localPosition = Vector3.zero;
            //     generateRobot.parts[i].transform.localRotation = Quaternion.identity;

            //     networkObject.GetComponent<MeshRenderer>().enabled = false;
            //     generateRobot.parts[i].GetComponent<MeshRenderer>().enabled = false;
            //     i++;
            // }

        }
        yield return null;
    }


    string FileToBase64(string filePath)
    {
        print(filePath);
        // ファイルが存在しない場合は例外をスローする
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"{filePath}指定されたファイルが見つかりません。", filePath);
        }

        // ファイルをバイト配列として読み込む
        byte[] fileBytes = File.ReadAllBytes(filePath);

        // バイト配列をBase64文字列に変換する
        string base64String = Convert.ToBase64String(fileBytes);



        return base64String;
    }

    // string FileToBase64(string filePath, out int hash)
    // {
    //     string base64String = FileToBase64(filePath);
    //     hash = FNV1a(base64String);
    // }
    int FNV1a(string file)
    {
        const int fnvPrime = 16777619;
        const int fnvOffsetBasis = unchecked((int)2166136261);
        int h = fnvOffsetBasis;

        foreach (char c in file)
        {
            h ^= c;
            h *= fnvPrime;
        }

        return Math.Abs(h);
    }
}
