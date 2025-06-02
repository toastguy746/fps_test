using UnityEngine;

public class ArmLook : MonoBehaviour
{
    public Transform playerCamera;

    private float lastCameraX;

    void Start()
    {
        if (playerCamera != null)
            lastCameraX = playerCamera.localEulerAngles.x;
    }

    void LateUpdate()
    {
        if (playerCamera == null) return;

        float currentCameraX = playerCamera.localEulerAngles.x;

        float deltaX = Mathf.DeltaAngle(lastCameraX, currentCameraX);

        Vector3 euler = transform.localEulerAngles;
        euler.y += deltaX;

        transform.localEulerAngles = euler;

        lastCameraX = currentCameraX;
    }
}
