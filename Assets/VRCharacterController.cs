using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRCharacterController : MonoBehaviour
{
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public GameObject xrOrigin;

    private XRNode activeHand = XRNode.LeftHand;
    private bool isClimbing = false;
    private Vector3 lastHandWorldPos;
    Rigidbody xrOriginRigidbody;
    void Start()
    {

        // Rigidbodyを取得
        xrOriginRigidbody = xrOrigin.GetComponent<Rigidbody>();

    }


    void Update()
    {
        climbing();
        moving();
    }
    public float moveSpeed;
    void moving()
    {
        InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (leftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 input))
        {
            Vector3 moveDirection = new Vector3(input.x, 0, input.y);

            // カメラの方向を考慮して向きを合わせる（ヘッドのY軸回転を使う）
            Transform headTransform = Camera.main.transform;
            Vector3 headYaw = new Vector3(headTransform.forward.x, 0, headTransform.forward.z).normalized;
            Quaternion headRotation = Quaternion.LookRotation(headYaw);

            Vector3 worldMove = headRotation * moveDirection * moveSpeed;

            xrOriginRigidbody.MovePosition(xrOriginRigidbody.position + worldMove * Time.deltaTime);
        }
    }

    void climbing()
    {
        InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        leftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool leftPressed);
        rightHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool rightPressed);

        // 登っていない → 左 or 右が押されたら開始
        if (!isClimbing)
        {
            if (leftPressed)
            {
                StartClimbing(XRNode.LeftHand);
            }
            else if (rightPressed)
            {
                StartClimbing(XRNode.RightHand);
            }
        }
        else
        {
            // 他の手が押されたら切り替える
            if (activeHand == XRNode.LeftHand && rightPressed)
            {
                StartClimbing(XRNode.RightHand);
            }
            else if (activeHand == XRNode.RightHand && leftPressed)
            {
                StartClimbing(XRNode.LeftHand);
            }

            // 登っている手の入力が離されたら終了
            InputDevice activeDevice = InputDevices.GetDeviceAtXRNode(activeHand);
            activeDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool activePressed);

            if (!activePressed)
            {
                StopClimbing();
            }
        }

        // 移動
        if (isClimbing)
        {
            Transform handTransform = GetHandTransform(activeHand);

            Vector3 delta = lastHandWorldPos - handTransform.position;
            xrOrigin.transform.position += delta * 2f;
            lastHandWorldPos = handTransform.position;
        }
    }



    void StartClimbing(XRNode hand)
    {
        activeHand = hand;
        isClimbing = true;
        lastHandWorldPos = GetHandTransform(hand).position;
        Debug.Log($"Climbing started with {hand}");

        if (xrOriginRigidbody != null)
        {
            xrOriginRigidbody.useGravity = false;  // 重力オフ
            xrOriginRigidbody.velocity = Vector3.zero;  // 速度リセット（必要に応じて）
        }
    }

    void StopClimbing()
    {
        isClimbing = false;

        if (xrOriginRigidbody != null)
        {
            xrOriginRigidbody.useGravity = true;  // 重力オン
        }

        Debug.Log("Climbing stopped");
    }

    Transform GetHandTransform(XRNode hand)
    {
        return hand == XRNode.LeftHand ? leftHandTransform : rightHandTransform;
    }
}
