using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Text;



public class generateRobot : MonoBehaviour
{
    // Start is called before the first frame update
    // public string filePath;
    public GLTFast.GltfAsset gltf;
    void Start()
    {

        // StartCoroutine(generate(File.ReadAllBytes(filePath)));


    }

    // public bool

    public GameObject partprefab;

    public IEnumerator generate(byte[] robotbyte, bool isServer = false, bool isOwner = true, string name = "robot")
    {
        // print("1");
        // byte[] robotbyte = File.ReadAllBytes(filePath);
        // print("2");
        yield return LoadGltfBinaryFromMemory(robotbyte);
        // print("3");
        yield return null;
        yield return GetChildObjects(this.transform);
        // while (true) yield return null;

        // if (isServer)
        {
            makeCollider makeCollider = this.gameObject.AddComponent<makeCollider>();
            yield return makeCollider.generate(objs);
            Destroy(makeCollider);
        }

        // yield return SetSimpleCollider();
        // yield return SetCollider();

        yield return null;
        yield return assembly(name);

        if (isServer) yield return massSet();



        yield return setActuator(isServer, isOwner);

        if (isServer)
        {

            foreach (GameObject part in parts)
            {
                Rigidbody rb = part.GetComponent<Rigidbody>();
                bool isfixed = false;
                foreach (Transform child in part.transform)
                {
                    if (child.gameObject.name.Contains("fixed")) isfixed = true;
                }

                if (isfixed == false) rb.isKinematic = false;
            }
        }



    }

    IEnumerator SetSimpleCollider()
    {

        foreach (GameObject obj in objs)
        {
            yield return null;
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;
            Mesh mesh = meshFilter.sharedMesh;

            var collider = obj.AddComponent<MeshCollider>();
            collider.sharedMesh = meshFilter.sharedMesh;
            collider.convex = true;
        }
    }

    // IEnumerator SetCollider()
    // {

    //     foreach (GameObject obj in objs)
    //     {
    //         yield return null;
    //         MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
    //         if (meshFilter == null) continue;
    //         Mesh mesh = meshFilter.sharedMesh;

    //         var collider = obj.AddComponent<MeshCollider>();
    //         collider.sharedMesh = meshFilter.sharedMesh;
    //         collider.convex = true;
    //     }

    //     foreach (GameObject obj in objs)
    //     {
    //         yield return null;
    //         MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
    //         if (meshFilter == null) continue;
    //         Mesh mesh = meshFilter.sharedMesh;
    //         if (mesh == null) continue;
    //         // (Vector3, Vector3, Vector3)[] triangles2 = new (Vector3, Vector3, Vector3)[mesh.triangles.Length / 3];
    //         // for (int j = 0; j < mesh.triangles.Length; j += 3)
    //         // {
    //         //     // 三角形の各頂点座標を取得
    //         //     Vector3 b1 = /*obj.transform.TransformPoint*/(mesh.vertices[mesh.triangles[j]]);
    //         //     Vector3 b2 = /*obj.transform.TransformPoint*/(mesh.vertices[mesh.triangles[j + 1]]);
    //         //     Vector3 b3 = /*obj.transform.TransformPoint*/(mesh.vertices[mesh.triangles[j + 2]]);

    //         //     // 頂点間に線を描画
    //         //     Debug.DrawLine(obj.transform.TransformPoint(b1), obj.transform.TransformPoint(b2), Color.red, 0.1f); // 線の描画色を赤に設定
    //         //     Debug.DrawLine(obj.transform.TransformPoint(b2), obj.transform.TransformPoint(b3), Color.red, 0.1f); // 線の描画色を赤に設定
    //         //     Debug.DrawLine(obj.transform.TransformPoint(b3), obj.transform.TransformPoint(b1), Color.red, 0.1f); // 線の描画色を赤に設定
    //         //     triangles2[j / 3] = (b1, b2, b3);
    //         //     Mesh triangleMesh = new Mesh();
    //         //     triangleMesh.vertices = new Vector3[] { b1, b2, b3 };
    //         //     triangleMesh.triangles = new int[] { 0, 1, 2 }; // 頂点インデックスを指定
    //         //     var collider = obj.AddComponent<MeshCollider>();
    //         //     collider.sharedMesh = triangleMesh;
    //         //     collider.convex = true;
    //         //     yield return new WaitForSeconds(0.1f);
    //         // }

    //         // 各面の隣接面を検索
    //         List<int> connected = new List<int>();
    //         connected.Add(0);

    //         List<int> addedList = new List<int>(); // すでに追加された三角形のインデックスを追跡するためのリスト
    //         while (true)
    //         {


    //             for (int j = 0; j < connected.Count; j++)
    //             {
    //                 for (int i = 0; i < mesh.triangles.Length; i += 3)
    //                 {
    //                     yield return null;
    //                     if (addedList.Contains(i)) continue;

    //                     // すでに追加された三角形でなく、かつ頂点を共有する場合に追加
    //                     if (CheckSharedVerticesCount(mesh, i, connected[j]))
    //                     {
    //                         connected.Add(i); // 隣接している三角形のインデックスを追加
    //                         print("追加");
    //                         addedList.Add(i); // 追加済みリストに追加
    //                         yield return null;
    //                         // connectedリスト内のすべての頂点を描画する
    //                         for (int k = 0; k < connected.Count; k += 1)
    //                         {
    //                             Vector3 b1 = mesh.vertices[mesh.triangles[connected[k]]];
    //                             Vector3 b2 = mesh.vertices[mesh.triangles[connected[k] + 1]];
    //                             Vector3 b3 = mesh.vertices[mesh.triangles[connected[k] + 2]];

    //                             Debug.DrawLine(obj.transform.TransformPoint(b1), obj.transform.TransformPoint(b2), Color.red, 0.1f); // 線の描画色を赤に設定
    //                             Debug.DrawLine(obj.transform.TransformPoint(b2), obj.transform.TransformPoint(b3), Color.red, 0.1f); // 線の描画色を赤に設定
    //                             Debug.DrawLine(obj.transform.TransformPoint(b3), obj.transform.TransformPoint(b1), Color.red, 0.1f); // 線の描画色を赤に設定
    //                         }
    //                     }

    //                     // yield return new WaitForSeconds(0.1f);
    //                 }
    //             }
    //             print("while");

    //         }


    //     }



    // }

    bool CheckSharedVerticesCount(Mesh mesh, int i, int j)
    {
        int sharedVerticesCount = 0;

        if (mesh.triangles[i] == mesh.triangles[j] ||
            mesh.triangles[i + 1] == mesh.triangles[j] ||
            mesh.triangles[i + 2] == mesh.triangles[j])
            sharedVerticesCount++;
        if (mesh.triangles[i] == mesh.triangles[j + 1] ||
            mesh.triangles[i + 1] == mesh.triangles[j + 1] ||
            mesh.triangles[i + 2] == mesh.triangles[j + 1])
            sharedVerticesCount++;
        if (mesh.triangles[i] == mesh.triangles[j + 2] ||
            mesh.triangles[i + 1] == mesh.triangles[j + 2] ||
            mesh.triangles[i + 2] == mesh.triangles[j + 2])
            sharedVerticesCount++;

        return sharedVerticesCount >= 2;
    }






    IEnumerator massSet()
    {
        foreach (GameObject part in parts)
        {
            Rigidbody rb = part.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        foreach (GameObject part in parts)
        {
            Rigidbody rb = part.GetComponent<Rigidbody>();

            Collider[] colliders = rb.GetComponentsInChildren<Collider>();

            // float totalVolume = 0f;

            rb.mass = 0;
            // 各Colliderのサイズから体積を計算し、合計体積を求める
            foreach (Collider collider in colliders)
            {
                rb.mass += CalculateVolume(collider.bounds.size);
            }
            yield return null;
        }
    }

    float CalculateVolume(Vector3 size)
    {
        return size.x * size.y * size.z;
    }

    IEnumerator NetCodepartReplace()
    {
        List<GameObject> NetCodeParts = new List<GameObject>();
        foreach (GameObject part in parts)
        {
            yield return null;
            // NetCodepartのインスタンスを生成
            GameObject instantiatedObject = Instantiate(partprefab, this.transform);

            // 各オブジェクトの位置、回転、スケールを親オブジェクトに合わせる
            instantiatedObject.transform.position = part.transform.position;
            instantiatedObject.transform.rotation = part.transform.rotation;
            instantiatedObject.transform.localScale = part.transform.localScale;
            // instantiatedObject.GetComponent<Unity.Netcode.NetworkObject>().Spawn();
            instantiatedObject.transform.SetParent(this.transform);
            List<Transform> children = new List<Transform>();
            foreach (Transform child in part.transform)
            {
                children.Add(child);
            }

            // 子オブジェクトを移動する
            foreach (Transform child in children)
            {
                child.SetParent(instantiatedObject.transform, false);
            }
            NetCodeParts.Add(instantiatedObject);
        }
        foreach (GameObject part in parts)
        {
            Destroy(part);
        }
        parts = NetCodeParts;
    }
    void OnDestroy()
    {
        foreach (GameObject part in parts)
        {
            Unity.Netcode.NetworkObject networkObject = part.GetComponent<Unity.Netcode.NetworkObject>();
            if (networkObject != null)
                networkObject.Despawn();

        }
    }
    public robotController robotController;
    IEnumerator setActuator(bool isServer, bool isOwner)
    {
        yield return null;

        foreach (var obj in objs)
        {
            // print(robotController + "," + obj.name);
            robotController.Device device = robotController.addDevice(obj.name);
            if (device != null) print("type:" + obj.name);
            // if (device != null) print("type::" + device.type);
            if (isServer && device != null && new string[] { "motor", "servo", "hinge" }.Contains(device.type))
            {
                GameObject morterBase = obj.transform.Find("morterBase").gameObject;
                GameObject morterBasePart = null;
                GameObject morterAxis = morterBase.transform.Find("morterAxis").gameObject;
                GameObject morterAxisPart = null;
                foreach (var part in parts)
                {
                    if (IsPositionInsideCollider(part, morterBase.transform.position))
                        morterBasePart = part;
                    if (IsPositionInsideCollider(part, morterAxis.transform.position))
                        morterAxisPart = part;
                }
                if (morterBasePart == morterAxisPart)
                {
                    print("hingeが一緒");
                    continue;
                }
                // print(morterBase + "," + morterAxis);
                // print(obj + "," + morterBasePart);
                if (morterBasePart != null && morterAxisPart != null)
                {
                    obj.transform.SetParent(morterBasePart.transform);

                    HingeJoint hingeJoint = morterBasePart.AddComponent<HingeJoint>();
                    hingeJoint.anchor = obj.transform.localPosition;
                    hingeJoint.axis = obj.transform.up;
                    hingeJoint.connectedBody = morterAxisPart.GetComponent<Rigidbody>();
                    Rigidbody connectedBody = hingeJoint.connectedBody;
                    hingeJoint.enableCollision = true;
                    // HingeJointのモーターを有効にする
                    // print(device.type);
                    if (new string[] { "motor", "servo" }.Contains(device.type))
                    {

                        hingeJoint.useMotor = true;
                        JointMotor motor = hingeJoint.motor;
                        motor.freeSpin = false;
                        motor.force = Mathf.Infinity;

                        hingeJoint.motor = motor;
                        if (device.type == "motor")
                        {

                            robotController.motor motorDevice = device as robotController.motor;
                            print(motorDevice.HingeJoint);
                            motorDevice.HingeJoint = hingeJoint;
                        }
                        else if (device.type == "servo")
                        {

                            robotController.servo servoDevice = device as robotController.servo;

                            // print(servoDevice.HingeJoint);
                            servoDevice.servoControl = hingeJoint.gameObject.AddComponent<servoControl>();
                            servoDevice.servoControl.HingeJoint = hingeJoint;
                            // servoControl.hingeJoint = hingeJoint;
                            // servoDevice.HingeJoint = hingeJoint;
                        }
                    }
                }


            }


            Camera cameraComponent = obj.GetComponent<Camera>();
            if (isOwner && cameraComponent != null && !cameraComponent.enabled)
            {
                cameraComponent.enabled = true;
                SetObjectParentToCollidingPart(obj, parts);
            }

            Light LightComponent = obj.GetComponent<Light>();
            if (LightComponent != null)
            {
                print("light");
                LightComponent.enabled = true;
                LightComponent.intensity /= 250f;
                SetObjectParentToCollidingPart(obj, parts);
                if (device != null && device.type == "light")
                {
                    robotController.light lightDevice = device as robotController.light;
                    lightDevice.lightComponent = obj.GetComponent<Light>();
                    // print("aa" + lightDevice.lightComponent);
                }
            }



            if (isOwner && device != null && device.type == "orbitCamera")
            {
                print("orbitcamera");
                SetObjectParentToCollidingPart(obj, parts);
                obj.AddComponent<orbitCameraInput>();
            }

        }

    }
    void SetObjectParentToCollidingPart(GameObject obj, List<GameObject> parts)
    {
        foreach (var part in parts)
        {
            if (IsPositionInsideCollider(part, obj.gameObject.transform.position))
            {
                obj.transform.SetParent(part.transform);
            }
        }
    }

    public bool IsPositionInsideCollider(GameObject rb, Vector3 pos)
    {
        // Rigidbody以下のすべての子オブジェクトにアタッチされているコライダーを取得
        Collider[] colliders = rb.GetComponentsInChildren<Collider>();

        // 各コライダーの境界ボックスをチェック
        foreach (Collider collider in colliders)
        {
            // 指定された位置が境界ボックス内に含まれているかどうかをチェック
            if (collider.bounds.Contains(pos))
            {
                return true;
            }
        }

        return false;
    }



    public List<GameObject> parts;
    public Dictionary<GameObject, Vector3> partspos;
    IEnumerator assembly(string name)
    {
        parts = new List<GameObject>();
        partspos = new Dictionary<GameObject, Vector3>();
        yield return null;


        List<List<GameObject>> partsLists = new List<List<GameObject>>();

        foreach (GameObject obj in objs)
        {
            if (obj.GetComponent<MeshFilter>() == null) continue;
            List<GameObject> partsList = new List<GameObject>();
            partsList.Add(obj.gameObject);

            partsLists.Add(partsList);
        }
        foreach (MeshCollider collider1 in colliders)
        {
            foreach (MeshCollider collider2 in colliders)
            {
                if (collider1 == collider2) continue;
                // print(collider1 + "," + collider2 + "=" + CheckCollision(collider1, collider2));
                if (CheckCollision(collider1, collider2))
                {
                    // PrintPartsLists(partsLists);
                    //  partsLists内のcollider1, collider2を含むものを統合する
                    var list1 = partsLists.FirstOrDefault(list => list.Contains(collider1.gameObject));
                    var list2 = partsLists.FirstOrDefault(list => list.Contains(collider2.gameObject));

                    if (list1 != list2 && list1 != null && list2 != null)
                    {
                        list1.AddRange(list2);
                        partsLists.Remove(list2);
                    }
                    // PrintPartsLists(partsLists);
                }
            }
        }

        print("------------------");
        int i = 0;
        foreach (var partsList in partsLists)
        {
            i += 1;
            // print("parts");
            GameObject part = Instantiate(partprefab);
            part.name = $"parts_{name}_{i}";
            part.tag = "parts";
            parts.Add(part);

            // コライダーの中心座標を計算する
            Vector3 colliderCenter = Vector3.zero;
            foreach (var obj in partsList)
            {
                Collider collider = obj.GetComponent<Collider>();
                if (collider != null)
                {
                    colliderCenter += collider.bounds.center;
                }
            }

            part.transform.position = colliderCenter;
            foreach (var obj in partsList)
            {
                // コライダーの中心座標に子オブジェクトを配置する
                obj.transform.SetParent(part.transform);
                // print(obj);
            }
            partspos.Add(part, part.transform.position);

            // print("----");
        }
        // print("------------------");
        // yield return new WaitForSeconds(10f);
        //     GameObject part = Instantiate(partprefab);

        //     part.transform.SetParent(this.transform);
        //     part.tag = "parts";
        //     collider.transform.SetParent(part.transform);
        //     parts.Add(part);

        // }


        // foreach (MeshCollider collider1 in colliders)
        // {
        //     foreach (MeshCollider collider2 in colliders)
        //     {
        //         if (collider1 == collider2) continue;
        //         print(CheckCollision(collider1, collider2));
        //         if (CheckCollision(collider1, collider2))
        //         {
        //             print(collider1.gameObject + "," + collider2.gameObject);
        //             foreach (Transform c in collider2.transform.parent)
        //             {
        //                 c.SetParent(collider1.transform.parent);
        //             }
        //         }
        //     }
        // }





        // foreach (GameObject part in parts)
        // {
        //     if (part.transform.childCount <= 0) Destroy(part);
        // }
        // yield return null;

        // parts.RemoveAll(part => part.ToString() == "null");


    }


    void PrintPartsLists(List<List<GameObject>> lists)
    {
        // print();
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("<color=blue>Printing Parts Lists:</color>");

        for (int i = 0; i < lists.Count; i++)
        {
            builder.AppendLine("<color=green>List " + i + ":</color>");
            foreach (var obj in lists[i])
            {
                builder.AppendLine(obj.ToString());
            }
        }

        Debug.Log(builder.ToString());
    }



    public List<MeshCollider> colliders = new List<MeshCollider>();
    public List<GameObject> objs = new List<GameObject>();
    // public List<HingeJoint> hinges = new List<HingeJoint>();


    IEnumerator GetChildObjects(Transform parent)
    {
        // 親オブジェクトの子の数だけループします
        foreach (Transform child in parent)
        {

            if (child.name != "loadprogress")
                objs.Add(child.gameObject);
            // 子オブジェクトを処理します
            // Debug.Log("Found child: " + child.name);



            yield return null;
            yield return GetChildObjects(child);
        }
    }



    public bool CheckCollision(MeshCollider meshCollider1, MeshCollider meshCollider2)
    {
        // メッシュが存在しない場合は衝突しないと判断する
        if (meshCollider1.sharedMesh == null || meshCollider2.sharedMesh == null)
        {
            return false;
        }
        // return false;




        //     // すべての三角形を描画
        //     (Vector3, Vector3, Vector3)[] triangles1 = new (Vector3, Vector3, Vector3)[mesh1.triangles.Length / 3];
        //     List<(Vector3, Vector3)> lines = new List<(Vector3, Vector3)>();
        //     for (int i = 0; i < mesh1.triangles.Length; i += 3)
        //     {
        //         // 三角形の各頂点座標を取得
        //         Vector3 a1 = meshCollider1.transform.TransformPoint(mesh1.vertices[mesh1.triangles[i]]);
        //         Vector3 a2 = meshCollider1.transform.TransformPoint(mesh1.vertices[mesh1.triangles[i + 1]]);
        //         Vector3 a3 = meshCollider1.transform.TransformPoint(mesh1.vertices[mesh1.triangles[i + 2]]);

        //         // 頂点間に線を描画
        //         Debug.DrawLine(a1, a2, Color.red, 10f); // 線の描画色を赤に設定
        //         Debug.DrawLine(a2, a3, Color.red, 10f); // 線の描画色を赤に設定
        //         Debug.DrawLine(a3, a1, Color.red, 10f); // 線の描画色を赤に設定
        //         triangles1[i / 3] = (a1, a2, a3);

        //     }
        //     // (Vector3, Vector3, Vector3)[] triangles2 = new (Vector3, Vector3, Vector3)[mesh2.triangles.Length / 3];
        //     // for (int j = 0; j < mesh2.triangles.Length; j += 3)
        //     // {
        //     //     // 三角形の各頂点座標を取得
        //     //     Vector3 b1 = meshCollider2.transform.TransformPoint(mesh2.vertices[mesh2.triangles[j]]);
        //     //     Vector3 b2 = meshCollider2.transform.TransformPoint(mesh2.vertices[mesh2.triangles[j + 1]]);
        //     //     Vector3 b3 = meshCollider2.transform.TransformPoint(mesh2.vertices[mesh2.triangles[j + 2]]);

        //     //     // 頂点間に線を描画
        //     //     Debug.DrawLine(b1, b2, Color.blue, 10f); // 線の描画色を赤に設定
        //     //     Debug.DrawLine(b2, b3, Color.blue, 10f); // 線の描画色を赤に設定
        //     //     Debug.DrawLine(b3, b1, Color.blue, 10f); // 線の描画色を赤に設定
        //     //     triangles2[j / 3] = (b1, b2, b3);
        //     // }




        //     for (int j = 0; j < mesh2.triangles.Length; j += 3)
        //     {
        //         // 直線の頂点座標を取得
        //         Vector3 b1 = meshCollider2.transform.TransformPoint(mesh2.vertices[mesh2.triangles[j]]);
        //         Vector3 b2 = meshCollider2.transform.TransformPoint(mesh2.vertices[mesh2.triangles[j + 1]]);
        //         Vector3 b3 = meshCollider2.transform.TransformPoint(mesh2.vertices[mesh2.triangles[j + 2]]);

        //         // 三角形の各辺を直線として追加（重複を避ける）
        //         AddUniqueLine(lines, b1, b2);
        //         AddUniqueLine(lines, b2, b3);
        //         AddUniqueLine(lines, b3, b1);
        //     }

        //     // 重複を追加しないように直線を追加する関数
        //     void AddUniqueLine(List<(Vector3, Vector3)> linesList, Vector3 point1, Vector3 point2)
        //     {
        //         // 既存の直線として追加しようとしている辺が存在しない場合にのみ追加
        //         if (!linesList.Contains((point1, point2)) && !linesList.Contains((point2, point1)))
        //         {
        //             linesList.Add((point1, point2));
        //         }
        //     }
        //     foreach (var line in lines)
        //     {
        //         foreach (var triangle in triangles1)
        //         {
        //             if (IntersectTriangle(line, triangle))
        //                 return true;
        //         }
        //     }

        //     // foreach (var t1 in triangles1)
        //     // {
        //     //     foreach (var t2 in triangles2)
        //     //     {
        //     //         if (TrianglesIntersect(t1, t2))
        //     //         {
        //     //             return true;
        //     //         }

        //     //     }
        //     // }
        // メッシュの境界ボックス同士が重なっているかを確認する
        if (!meshCollider1.bounds.Intersects(meshCollider2.bounds))
        {
            return false;
        }

        //  実際にメッシュ同士が交差しているかどうかを判定する
        Mesh mesh1 = meshCollider1.sharedMesh;
        Mesh mesh2 = meshCollider2.sharedMesh;

        Vector3[] vertices1 = mesh1.vertices;
        Vector3[] vertices2 = mesh2.vertices;

        // 一方のメッシュの頂点がもう一方のメッシュの内部にあるかを確認する
        for (int i = 0; i < vertices1.Length; i++)
        {
            Vector3 point = meshCollider1.transform.TransformPoint(vertices1[i]);
            if (meshCollider2.bounds.Contains(point))
            {
                return true;
            }
        }

        for (int i = 0; i < vertices2.Length; i++)
        {
            Vector3 point = meshCollider2.transform.TransformPoint(vertices2[i]);
            if (meshCollider1.bounds.Contains(point))
            {
                return true;
            }
        }

        // メッシュ同士が交差していない場合は衝突しないと判断する
        return false;
    }
    // bool IntersectTriangle((Vector3, Vector3) line, (Vector3, Vector3, Vector3) triangle)
    // {
    //     Vector3 edge1 = triangle.Item2 - triangle.Item1;
    //     Vector3 edge2 = triangle.Item3 - triangle.Item1;
    //     Vector3 h = Vector3.Cross(line.Item2 - line.Item1, edge2);
    //     float a = Vector3.Dot(edge1, h);
    //     if (a > -Mathf.Epsilon && a < Mathf.Epsilon)
    //         return false;
    //     float f = 1.0f / a;
    //     Vector3 s = line.Item1 - triangle.Item1;
    //     float u = f * Vector3.Dot(s, h);
    //     if (u < 0.0f || u > 1.0f)
    //         return false;
    //     Vector3 q = Vector3.Cross(s, edge1);
    //     float v = f * Vector3.Dot(line.Item2 - triangle.Item1, q);
    //     if (v < 0.0f || u + v > 1.0f)
    //         return false;
    //     float t = f * Vector3.Dot(edge2, q);
    //     if (t > Mathf.Epsilon)
    //         return true;
    //     return false;
    // }

    // private bool TrianglesIntersect((Vector3, Vector3, Vector3) triangle1, (Vector3, Vector3, Vector3) triangle2)
    // {
    //     // triangle1とtriangle2の各辺を構成する頂点を取得
    //     Vector3[] tri1Vertices = new Vector3[] { triangle1.Item1, triangle1.Item2, triangle1.Item3 };
    //     Vector3[] tri2Vertices = new Vector3[] { triangle2.Item1, triangle2.Item2, triangle2.Item3 };

    //     // 交差を判定するための基準となるベクトル
    //     Vector3 epsilon = new Vector3(0.000001f, 0.000001f, 0.000001f);

    //     for (int i = 0; i < 3; i++)
    //     {
    //         Vector3 edge1 = tri1Vertices[(i + 1) % 3] - tri1Vertices[i];
    //         Vector3 edge2 = tri2Vertices[1] - tri2Vertices[0];
    //         Vector3 h = Vector3.Cross(edge2, tri1Vertices[i] - tri2Vertices[0]);
    //         float a = Vector3.Dot(edge1, h);

    //         // 2つの平面が平行である場合
    //         if (a > -epsilon.magnitude && a < epsilon.magnitude)
    //             continue;

    //         float f = 1.0f / a;
    //         Vector3 s = tri1Vertices[i] - tri2Vertices[0];
    //         float u = f * Vector3.Dot(s, h);

    //         if (u < 0 || u > 1)
    //             continue;

    //         Vector3 q = Vector3.Cross(s, edge1);
    //         float v = f * Vector3.Dot(edge2, q);

    //         if (v < 0 || u + v > 1)
    //             continue;

    //         float t = f * Vector3.Dot(edge2, q);

    //         if (t > epsilon.magnitude)
    //             return true; // 交差している場合

    //     }

    //     return false; // 交差していない場合
    // }







    // public GltfAsset gltfAsset;
    private IEnumerator LoadGltfBinaryFromMemory(byte[] data)
    {
        var gltf = new GltfImport();
        Task<bool> loadTask = gltf.LoadGltfBinary(data);

        // 非同期タスクの完了を待ちます
        yield return new WaitUntil(() => loadTask.IsCompleted);

        // タスクの結果を取得し、成功した場合はインスタンス化を試みます
        bool success = loadTask.Result;
        if (success)
        {
            Task<bool> instantiateTask = gltf.InstantiateMainSceneAsync(transform);

            // インスタンス化の非同期タスクの完了を待ちます
            yield return new WaitUntil(() => instantiateTask.IsCompleted);

            // インスタンス化が成功したかどうかを確認します
            bool instantiateSuccess = instantiateTask.Result;
            if (!instantiateSuccess)
            {
                print("Failed to instantiate main scene.");
            }
        }
        else
        {
            print("Failed to load glTF binary.");
        }
    }
    // Update is called once per frame
    void Update()
    {

    }


    // glTFファイルのURL
    //file://Users/taken/projects/VirtualRobot4/Assets/robots/test1.glb

    /*
    C:\Users\taken\projects\VirtualRobot4\Assets\robots\test1.glb



    */


}







/*


import bpy

class ExportGLTF(bpy.types.Operator):
    bl_idname = "object.export_gltf"
    bl_label = "Export as glTF"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        print("Exporting as glTF")
        export_settings = {
           
            "export_cameras": True,  # カメラをエクスポートするかどうか
            "export_lights": True,   # ライトをエクスポートするかどうか
        }
        bpy.ops.export_scene.gltf(filepath="./robot.gltf", **export_settings)
        return {'FINISHED'}

class VirtualRobotPanel(bpy.types.Panel):
    bl_label = "VirtualRobot4forBlender"
    bl_idname = "PT_VirtualRobotPanel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'VirtualRobot4forBlender'

    def draw(self, context):
        layout = self.layout
        layout.operator("object.export_gltf", text="Export as glTF")

def register():
    bpy.utils.register_class(ExportGLTF)
    bpy.utils.register_class(VirtualRobotPanel)

def unregister():
    bpy.utils.unregister_class(ExportGLTF)
    bpy.utils.unregister_class(VirtualRobotPanel)

if __name__ == "__main__":
    register()
    

*/