using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps : MonoBehaviour
{
    public float flySpeed = 5f; // カメラの移動速度
    public float rotationSpeed = 2f; // カメラの回転速度
    private bool cursorLockRequested = true;
    private bool wasActiveLastFrame = false;

    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        bool isActive = (cameraUI.activeCamera != null && cameraUI.activeCamera.transform == c);

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

            if (cursorLockRequested && netWorkUI.pause == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (Cursor.lockState != CursorLockMode.Locked) return;

            // WASDキーの入力に応じてカメラを移動させる
            // float moveHorizontal = Input.GetAxis("Horizontal");
            // float moveVertical = Input.GetAxis("Vertical");


            float moveHorizontal = 0f;
            float moveVertical = 0f;
            if (Input.GetKey(KeyCode.W))
                moveVertical = 1f;
            if (Input.GetKey(KeyCode.S))
                moveVertical = -1f;
            if (Input.GetKey(KeyCode.D))
                moveHorizontal = 1f;
            if (Input.GetKey(KeyCode.A))
                moveHorizontal = -1f;


            Vector3 moveDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized * (Input.GetKey(KeyCode.LeftControl) ? 3 : 1);

            transform.Translate(moveDirection * flySpeed * Time.deltaTime);

            // Shiftキーでカメラを上昇させる
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(Vector3.down * flySpeed * Time.deltaTime);
            }

            // Spaceキーでカメラを下降させる
            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(Vector3.up * flySpeed * Time.deltaTime);
            }

            // マウスの移動に応じてカメラを回転させる
            mouseX += Input.GetAxis("Mouse X");
            mouseY += Input.GetAxis("Mouse Y");

            transform.localEulerAngles = new Vector3(0, mouseX, 0);
            c.localEulerAngles = new Vector3(-mouseY, 0, 0);
        }
        else if (wasActiveLastFrame)
        {
            // I was active, but now I'm not. Relinquish cursor control.
            Cursor.lockState = CursorLockMode.None;
            wasActiveLastFrame = false;
        }
    }
    public float mouseX;
    public float mouseY;
    public Transform c;
}
