using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

using UnityEngine;

public class orbitCameraInput : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform y;
    public Camera c;
    void Start()
    {
        y = new GameObject().transform;
        y.SetParent(this.transform);
        y.localPosition = new Vector3(0, 0, 0);
        y.localRotation = Quaternion.identity;
        c = new GameObject().transform.gameObject.AddComponent<Camera>();
        c.transform.SetParent(y);
        distance = 15;
        c.transform.localPosition = new Vector3(0, 0, -distance);
        c.transform.localRotation = Quaternion.identity;
    }
    float mouseX;
    float mouseY;
    float distance;

    // Update is called once per frame
    void Update()
    {


        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (cameraUI.activeCamera != c) return;
        mouseX += Input.GetAxis("Mouse X");
        mouseY += Input.GetAxis("Mouse Y");

        transform.localEulerAngles = new Vector3(0, mouseX, 0);
        y.localEulerAngles = new Vector3(-mouseY, 0, 0);
        distance -= Input.GetAxis("Mouse ScrollWheel") * 0.8f;
        if (distance < 0) distance = 0;

        c.transform.localPosition = new Vector3(0, 0, -distance);
        c.transform.LookAt(this.transform);
    }
}
