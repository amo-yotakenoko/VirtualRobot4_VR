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

        var cameraObject = new GameObject();

        cameraObject.tag = "playerCamera";

        c = cameraObject.transform.gameObject.AddComponent<Camera>();
        c.transform.SetParent(y);
        distance = 15;
        c.transform.localPosition = new Vector3(0, 0, -distance);
        c.transform.localRotation = Quaternion.identity;
    }
    float mouseX;
    float mouseY;
    float distance;
    private bool cursorLockRequested = true;
    private bool wasActiveLastFrame = false;

    // Update is called once per frame
    void Update()
    {
        bool isActive = (cameraUI.activeCamera == c);

        if (isActive)
        {
            wasActiveLastFrame = true;

            // This is the active camera, so manage cursor and input.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLockRequested = false;
            }
            // Re-lock the cursor only if the game is not paused and not clicking on a UI element.
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && netWorkUI.pause == false)
            {
                cursorLockRequested = true;
            }

            if (cursorLockRequested)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (Cursor.lockState != CursorLockMode.Locked) return;

            mouseX += Input.GetAxis("Mouse X");
            mouseY += Input.GetAxis("Mouse Y");

            transform.localEulerAngles = new Vector3(0, mouseX, 0);
            y.localEulerAngles = new Vector3(-mouseY, 0, 0);
            distance -= Input.GetAxis("Mouse ScrollWheel") * 0.8f;
            if (distance < 0) distance = 0;

            c.transform.localPosition = new Vector3(0, 0, -distance);
            c.transform.LookAt(this.transform);
        }
        else if (wasActiveLastFrame)
        {
            // I was active, but now I'm not. Relinquish cursor control.
            Cursor.lockState = CursorLockMode.None;
            wasActiveLastFrame = false;
        }
    }
}
