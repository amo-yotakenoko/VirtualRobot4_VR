using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps : MonoBehaviour
{
    public float flySpeed = 5f; // カメラの移動速度
    public float rotationSpeed = 2f; // カメラの回転速度

    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (cameraUI.activeCamera == null || cameraUI.activeCamera.transform != c) return;
        // Cursor.lockState = CursorLockMode.None;



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
        Vector3 moveDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

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
    public float mouseX;
    public float mouseY;
    public Transform c;
}
