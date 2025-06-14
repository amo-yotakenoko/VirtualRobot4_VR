using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSensor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = SpawnDistanceLine();
    }
    LineRenderer lineRenderer;

    public const float maxDistance = 100f;


    float transparent = 1f; // 透明度の値（0.0f から 1.0f の範囲）
    void Update()
    {



        transparent = Math.Clamp(transparent - Time.deltaTime * 2, 0f, 1f);
        if (transparent > 0f)
        {
            lineRenderer.gameObject.SetActive(true);
            lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, transparent));
        }
        else
        {

            lineRenderer.gameObject.SetActive(false);
        }

    }

    public LineRenderer SpawnDistanceLine()
    {
        // Resources フォルダから distanceLine プレハブを読み込む
        GameObject prefab = Resources.Load<GameObject>("distanceLine");


        // 自分の子としてインスタンス化
        GameObject instance = Instantiate(prefab, transform);

        // LineRenderer コンポーネントを取得して返す
        LineRenderer lineRenderer = instance.GetComponent<LineRenderer>();


        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, new Vector3(0, 5, 0));

        return lineRenderer;
    }


    public float getDistance()
    {
        Vector3 origin = transform.position;
        Vector3 direction = this.transform.up;
        Vector3 endPoint;

        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, maxDistance))
        {
            endPoint = hitInfo.point;
        }
        else
        {
            endPoint = origin + direction * maxDistance;
        }
        float distance = Vector3.Distance(origin, endPoint);
        lineRenderer.SetPosition(1, new Vector3(0, distance, 0));
        transparent = 1;
        return distance;
    }
}
