using UnityEngine;

public class ArmLook : MonoBehaviour
{
    public Transform playerCamera;  // 플레이어 시점 카메라

    private float lastCameraX;      // 이전 프레임 카메라 X축 회전값 저장

    void Start()
    {
        if (playerCamera != null)
            lastCameraX = playerCamera.localEulerAngles.x;  // 초기값 세팅
    }

    void LateUpdate()
    {
        if (playerCamera == null) return;  // 카메라 없으면 작업 중단

        float currentCameraX = playerCamera.localEulerAngles.x;  // 현재 카메라 X 회전값

        float deltaX = Mathf.DeltaAngle(lastCameraX, currentCameraX);  // 이전과 현재 각도 차이 계산

        Vector3 euler = transform.localEulerAngles;  
        euler.y += deltaX;  // 팔의 Y축 회전에 카메라 X축 회전 변화량 적용

        transform.localEulerAngles = euler;  // 회전값 반영

        lastCameraX = currentCameraX;  // 현재 각도를 저장해 다음 프레임에 대비
    }
}
