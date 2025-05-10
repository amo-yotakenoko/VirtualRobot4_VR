using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class makeCollider : MonoBehaviour
{
    public class Triangle
    {
        public Vector3 v0;
        public Vector3 v1;
        public Vector3 v2;


        public Triangle(Vector3 ve0, Vector3 ve1, Vector3 ve2)
        {
            v0 = ve0;
            v1 = ve1;
            v2 = ve2;
        }
    }
    // Start is called before the first frame update
    // void Start()
    // {
    //     StartCoroutine(generate());

    // }

    // Update is called once per frame
    void Update()
    {
        progressMeshDraw();
    }
    public List<GameObject> objs = new List<GameObject>();
    public List<MeshCollider> colliders = new List<MeshCollider>();




    // public Material tmpmaterial;

    public int runningCoroutineCount = 0;

    public IEnumerator generate(List<GameObject> generateobjs)
    {
        objs = generateobjs;
        // print("1");
        // byte[] robotbyte = File.ReadAllBytes(filePath);
        // print("2");

        // yield return GetChildObjects(this.transform);

        // while (true) yield return null;

        // foreach (GameObject obj in objs)
        // {
        //     MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        //     if (meshFilter == null) continue;
        //     var r = obj.AddComponent<Rigidbody>();
        //     obj.AddComponent<MeshCollider>().convex = true;

        // }

        // List<Coroutine> coroutines = new List<Coroutine>();
        // runningCoroutineCount = 0;
        colliders = new List<MeshCollider>();
        List<MeshRenderer> hidedmeshs = new List<MeshRenderer>();
        foreach (GameObject obj in objs)
        {



            // yield return AddNotConvoxCollider(obj);
            // yield return AddSplitNotConvoxCollider(obj);
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;
            runningCoroutineCount += 1;
            StartCoroutine(AddSplitNotConvoxCollider(obj));
            obj.GetComponent<MeshRenderer>().enabled = false;
            hidedmeshs.Add(obj.GetComponent<MeshRenderer>());


            // yield return AddSplitNotConvoxCollider(obj);
            // var r = obj.AddComponent<Rigidbody>();
            // yield return null;
            // yield return null;
            // yield return null;
        }

        while (runningCoroutineCount > 0)
        {

            yield return null;
        }
        // yield return new WaitForSeconds(1);
        foreach (MeshRenderer hidedmesh in hidedmeshs)
        {
            hidedmesh.enabled = true;
        }



    }
    List<Material> transparentMaterials = new List<Material>();
    void progressMeshDraw()
    {
        // UnityEngine.Random.InitState(0);

        // 0 から 1 の間の乱数を生成して色の RGB 成分に適用


        // 生成した RGB 成分を使って新しい色を作成
        // Color newColor = new Color(Random.value, Random.value, Random.value);
        int i = 0;
        foreach (MeshCollider collider in colliders)
        {
            Mesh mesh = collider.sharedMesh;
            if (mesh != null)
            {
                Vector3 position = collider.transform.position;
                Vector3 scale = collider.transform.lossyScale;
                Vector3 rotation = collider.transform.rotation.eulerAngles;

                Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
                Color c = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0.9f, 1f), UnityEngine.Random.Range(0.5f, 1f));
                if (transparentMaterials.Count < i + 1) transparentMaterials.Add(CreateTransparentMaterial(c, 0.2f));


                Graphics.DrawMesh(mesh, matrix, transparentMaterials[i], 0);
            }
            i += 1;
        }
    }
    public Material transparentMaterial;
    public Material CreateTransparentMaterial(Color color, float alpha)
    {
        // print(transparentMaterial);
        if (transparentMaterial == null) transparentMaterial = (Material)Resources.Load("transparentMaterial");
        // 新しい Material を作成
        // Material material = new Material(Shader.Find("Standard"));
        // material.SetFloat("_Mode", 3); // Fade モード
        // material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        // material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // material.SetInt("_ZWrite", 0);
        // material.DisableKeyword("_ALPHATEST_ON");
        // material.EnableKeyword("_ALPHABLEND_ON");
        // material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        // material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        Material material = new Material(transparentMaterial);
        color.a = alpha;
        material.color = color;


        return material;
    }

    // public List<Triangle> triangles = new List<Triangle>();
    IEnumerator AddSplitNotConvoxCollider(GameObject obj)
    {

        List<Triangle> triangles = new List<Triangle>();
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null) yield break;
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        List<int> allAddedTriangleIds = new List<int>();
        // addedTriangleIds.Add(0);
        // List<Vector3> addedVertices = new List<Vector3>();
        HashSet<Vector3> addedVertices = new HashSet<Vector3>();
        // addedVertices.Add(mesh.vertices[mesh.triangles[0]]);
        int splitcount = 0;
        while (allAddedTriangleIds.Count != mesh.triangles.Length / 3)
        {

            splitcount += 1;
            for (int i = 0; i < mesh.triangles.Length; i += 1)//ないものを追加
            {
                if (!addedVertices.Contains(mesh.vertices[mesh.triangles[i]]))
                {

                    addedVertices.Add(mesh.vertices[mesh.triangles[i]]);
                    break;
                }
            }


            yield return null;
            // List<int> triangleIndices = new List<int>();//newmesh用
            // List<Vector3> triangleVertices = new List<Vector3>();
            List<Triangle> meshTrangles = new List<Triangle>();
            Mesh splitMesh = new Mesh();
            int count = 0;
            while (true)
            {
                bool end = true;
                for (int i = 0; i < mesh.triangles.Length; i += 3)
                {
                    if (count++ % 100 == 0)
                        yield return null;
                    if (allAddedTriangleIds.Contains(i)) continue;

                    // print(addedVertices.Count);
                    // print(mesh.triangles[i] + "," + mesh.triangles[i + 1] + "," + mesh.triangles[i + 2]);
                    // 新しいメッシュを作成
                    if (addedVertices.Contains(mesh.vertices[mesh.triangles[i]]) ||
                    addedVertices.Contains(mesh.vertices[mesh.triangles[i + 1]]) ||
                    addedVertices.Contains(mesh.vertices[mesh.triangles[i + 2]]))
                    {
                        // print("ついか");

                        if (!addedVertices.Contains(mesh.vertices[mesh.triangles[i]]))
                        {
                            addedVertices.Add(mesh.vertices[mesh.triangles[i]]);
                            end = false;
                        }
                        if (!addedVertices.Contains(mesh.vertices[mesh.triangles[i + 1]]))
                        {
                            addedVertices.Add(mesh.vertices[mesh.triangles[i + 1]]);
                            end = false;
                        }
                        if (!addedVertices.Contains(mesh.vertices[mesh.triangles[i + 2]]))
                        {
                            addedVertices.Add(mesh.vertices[mesh.triangles[i + 2]]);
                            end = false;
                        }


                        // triangleIndices.Add(i);
                        // triangleIndices.Add(i + 1);
                        // triangleIndices.Add(i + 2);
                        // triangleVertices.Add(mesh.vertices[mesh.triangles[i]]);
                        // triangleVertices.Add(mesh.vertices[mesh.triangles[i + 1]]);
                        // triangleVertices.Add(mesh.vertices[mesh.triangles[i + 2]]);
                        Triangle tri = new Triangle(mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i + 1]], mesh.vertices[mesh.triangles[i + 2]]);
                        meshTrangles.Add(tri);
                        allAddedTriangleIds.Add(i);
                        // print(addedTriangleIds.Count);

                        // Vector3 v0 = obj.transform.TransformPoint(vertices[mesh.triangles[i]]);
                        // Vector3 v1 = obj.transform.TransformPoint(vertices[mesh.triangles[i + 1]]);
                        // Vector3 v2 = obj.transform.TransformPoint(vertices[mesh.triangles[i + 2]]);
                        Debug.DrawLine(obj.transform.TransformPoint(tri.v0), obj.transform.TransformPoint(tri.v1), Color.red, 1.0f);
                        Debug.DrawLine(obj.transform.TransformPoint(tri.v0), obj.transform.TransformPoint(tri.v2), Color.red, 1.0f);
                        Debug.DrawLine(obj.transform.TransformPoint(tri.v1), obj.transform.TransformPoint(tri.v2), Color.red, 1.0f);

                        // yield return new WaitForSeconds(0.1f);
                    }


                }

                if (end) break;
            }

            List<Vector3> triangleVertices = new List<Vector3>();

            foreach (var triangle in meshTrangles)
            {
                triangleVertices.Add(triangle.v0);
                triangleVertices.Add(triangle.v1);
                triangleVertices.Add(triangle.v2);
            }
            triangleVertices = triangleVertices.Distinct().ToList();

            List<int> triangleIndices = new List<int>();//newmesh用
            foreach (var tri in meshTrangles)
            {
                triangleIndices.Add(triangleVertices.IndexOf(tri.v0));
                triangleIndices.Add(triangleVertices.IndexOf(tri.v1));
                triangleIndices.Add(triangleVertices.IndexOf(tri.v2));
            }


            splitMesh.vertices = triangleVertices.ToArray();
            splitMesh.triangles = triangleIndices.ToArray();


            // var collider = obj.AddComponent<MeshCollider>();
            // collider.sharedMesh = splitMesh;
            // collider.convex = false;

            // print(allAddedTriangleIds.Count + "/" + mesh.triangles.Length / 3);
            yield return AddNotConvoxCollider(obj, splitMesh, triangles);



        }
        runningCoroutineCount -= 1;

        // newMesh.RecalculateNormals(); // 法線を再計算する
        // // MeshFilterに新しいMeshを設定する

    }







    IEnumerator AddNotConvoxCollider(GameObject obj, Mesh mesh, List<Triangle> triangles)
    {
        // print("AddNotConvoxCollider");
        var collider = obj.AddComponent<MeshCollider>();
        colliders.Add(collider);
        collider.convex = true;
        // yield break;
        if (!convoxError(obj, mesh, collider)) yield break;




        // int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        triangles.Clear();
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            // 新しいメッシュを作成



            Vector3 v0 = vertices[mesh.triangles[i]];
            Vector3 v1 = vertices[mesh.triangles[i + 1]];
            Vector3 v2 = vertices[mesh.triangles[i + 2]];
            triangles.Add(new Triangle(v0, v1, v2));
        }
        // triangles.Sort((x, y) => UnityEngine.Random.Range(-1, 2));
        // collider = obj.AddComponent<MeshCollider>();
        List<int> triangleIndices = new List<int>();
        List<Vector3> triangleVertices = new List<Vector3>();
        int count = 0;
        while (triangles.Count > 0)
        {
            count++;
            if (count > 100) break;
            yield return null;
            bool end = true;
            for (int i = 0; i < triangles.Count; i++)
            {
                // yield return null;
                Triangle triangle = triangles[i];
                Mesh triangleMesh = new Mesh();

                triangleVertices.Add(triangle.v0);
                triangleVertices.Add(triangle.v1);
                triangleVertices.Add(triangle.v2);

                triangleIndices.Add(triangleIndices.Count);
                triangleIndices.Add(triangleIndices.Count);
                triangleIndices.Add(triangleIndices.Count);


                triangleMesh.vertices = triangleVertices.ToArray();
                triangleMesh.triangles = triangleIndices.ToArray();

                if (triangleMesh.vertices.Length <= 4) continue;
                // print(triangleMesh.vertices.Length);
                if (triangleVertices.ToArray().Length > 3 && triangleIndices.ToArray().Length > 3)
                {

                    collider.sharedMesh = triangleMesh;
                    collider.convex = true;


                }
                // yield return null;
                // yield return new WaitForSeconds(0.1f);
                // ここで条件に基づいて要素を削除するかどうかをチェック
                if (convoxError(obj, mesh, collider))
                {
                    triangleMesh = new Mesh();
                    triangleVertices.RemoveRange(triangleVertices.Count - 3, 3);
                    triangleIndices.RemoveRange(triangleIndices.Count - 3, 3);

                    // print(triangleVertices.ToArray().Length + "," + triangleIndices.ToArray().Length);
                    if (triangleVertices.ToArray().Length > 3 && triangleIndices.ToArray().Length > 3)
                    {

                        triangleMesh.vertices = triangleVertices.ToArray();
                        triangleMesh.triangles = triangleIndices.ToArray();
                        triangleMesh.RecalculateNormals();
                        collider.sharedMesh = triangleMesh;
                        collider.convex = true;
                    }
                }
                else
                {
                    triangles.RemoveAt(i);
                    i--;
                    // print(triangles.Count);
                    end = false;
                    yield return null;
                }
                // yield return new WaitForSeconds(1f);
            }
            if (end)
            {
                // yield return new WaitForSeconds(2f);
                // Destroy(collider);
                yield return null;
                triangleIndices.Clear();
                triangleVertices.Clear();
                colliders.Add(collider);
                collider = obj.AddComponent<MeshCollider>();
            }

        }
    }

    bool convoxError(GameObject obj, Mesh mesh, MeshCollider collider)
    {
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // 新しいメッシュを作成

            Vector3 v0 = obj.transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = obj.transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = obj.transform.TransformPoint(vertices[triangles[i + 2]]);
            Vector3 c = (v0 + v1 + v2) / 3f;
            List<Vector3> centers = new List<Vector3>
        {
            c,
            (c+v0)/2.0f,
            (c+v1)/2.0f,
            (c+v2)/2.0f,
            // v0,
            // v1,
            // v2,
            //  (v0 + v1+ v2 ) / 3f,
            // (v0 * 0.8f + v1 * 0.1f + v2 * 0.1f) / 3f,
            // (v0 * 0.1f + v1 * 0.8f + v2 * 0.1f) / 3f,
            // (v0 * 0.1f + v1 * 0.1f + v2 * 0.8f) / 3f
        };
            foreach (var center in centers)
            {


                // Vector3 center = (v0 + v1 + v2) / 3f;
                // Vector3 normal = mesh.normals[i];
                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v1).normalized * 0.09f;
                // Vector3 end = center + normal * 0.5f;


                // DrawLineで法線を描画
                Debug.DrawLine(v0, v1, Color.blue, 1.0f);
                Debug.DrawLine(v0, v2, Color.blue, 1.0f);
                Debug.DrawLine(v1, v2, Color.blue, 1.0f);
                // Debug.DrawLine(v0, center, Color.green, 1.0f);
                // Debug.DrawLine(v1, center, Color.green, 1.0f);
                // Debug.DrawLine(v2, center, Color.green, 1.0f);


                Debug.DrawLine(center, center + normal, Color.white, 1.0f);
                // Debug.DrawLine(center, center - normal, Color.white, 1.0f);
                // Debug.DrawLine(collider.ClosestPoint(end), end, Color.white, 1.0f);
                if (collider.ClosestPoint(center + normal) == center + normal)
                {
                    // print("エラー");
                    // Debug.DrawLine(center, center + normal, Color.red, 1.0f);
                    // Debug.DrawLine(center, center - normal, Color.red, 1.0f);
                    // Debug.DrawLine(center, end, Color.red, 10f);
                    return true;
                }
            }
        }
        return false;
    }

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
}
