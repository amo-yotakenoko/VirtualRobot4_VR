using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propertyObj : MonoBehaviour
{
    // Start is called before the first frame update
    public robotController.Device device;
    TMPro.TextMeshPro text;
    void Start()
    {
        text = GetComponent<TMPro.TextMeshPro>();


    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"{device.toString()}";
        transform.position = device.transform.position;
        Vector3 directionToCamera = cameraUI.activeCamera.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
        transform.rotation = targetRotation;
        transform.Rotate(0, 180f, 0); // 正面をカメラに向ける（逆を向く場合）
        // mainCamera = Camera.main;
    }
}
