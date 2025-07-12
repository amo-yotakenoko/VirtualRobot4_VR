using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour, stageObject
{
    public robotController.Device device;
    public void Start()
    {
        pointCounter.stageObjects.Add(this);


    }

    private HashSet<GameObject> touchingPoints = new HashSet<GameObject>();

    void OnCollisionEnter(Collision collision)
    {
        GameObject parent = GetRigidbodyRoot(collision.gameObject);

        if (parent != null && HasPointChild(parent))
        {
            touchingPoints.Add(parent);
            Debug.Log($"接触中: {touchingPoints.Count}個の point を含むオブジェクト");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        GameObject parent = GetRigidbodyRoot(collision.gameObject);

        if (parent != null && HasPointChild(parent))
        {
            touchingPoints.Remove(parent);
            Debug.Log($"接触中: {touchingPoints.Count}個の point を含むオブジェクト");
        }
    }

    GameObject GetRigidbodyRoot(GameObject obj)
    {
        // Rigidbody を持つ親オブジェクトをたどって探す
        Transform current = obj.transform;
        while (current != null)
        {
            if (current.GetComponent<Rigidbody>() != null)
            {
                return current.gameObject;
            }
            current = current.parent;
        }
        return null;
    }

    bool HasPointChild(GameObject parent)
    {
        // 親オブジェクトの子をすべて調べて "point" を含む名前があるかチェック
        foreach (Transform child in parent.transform)
        {
            if (child.name.Contains("point"))
            {
                return true;
            }
        }
        return false;
    }


    public int pointCount()
    {
        touchingPoints.RemoveWhere(obj => obj == null);
        return touchingPoints.Count;
    }
    public string getTeamName()
    {
        return device.name;
    }
}
