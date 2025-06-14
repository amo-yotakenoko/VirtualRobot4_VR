using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
public class viewProperty : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public bool enable;
    private bool previousEnableState = false;
    void Update()
    {
        if (enable != previousEnableState)
        {
            print("enable変更: " + enable);
            previousEnableState = enable;

            if (enable)
            {

                EnableView(GetAllPlayerDevices());
            }
            else
            {
                DisableView();
            }
        }
    }


    /* ─────────────── ② TMP 生成/削除ロジック ─────────────── */

    public GameObject TMPObject;     // TextMeshPro のプレハブ
    private readonly List<GameObject> _spawned = new();

    public void EnableView(List<robotController.Device> deviceList)
    {
        foreach (var device in deviceList)
        {
            // if (device == null || device.transform == null) continue;

            // 表示用TMPを複製
            var go = Instantiate(TMPObject, device.transform.position, Quaternion.identity);
            go.transform.SetParent(device.transform);               // デバイスに追従

            var tmp = go.GetComponent<TMPro.TextMeshPro>();
            if (tmp) tmp.text = $"{device.type}\n{device.name}";
            print($"{device.type}\n{device.name}");

            _spawned.Add(go);
        }
    }

    public void DisableView()
    {
        foreach (var go in _spawned)
            if (go) Destroy(go);
        _spawned.Clear();
    }

    public List<robotController.Device> GetAllPlayerDevices()
    {
        var robots = Object.FindObjectsOfType<robotController>(true);
        return robots
          .Where(r => r != null && r.deviceList != null)
          .SelectMany(r => r.deviceList)
          .ToList();
    }
}
