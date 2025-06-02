using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 150f;   // 마우스 감도 조절용 변수
    public Transform playerBody;             // 플레이어 몸통(오브젝트) 참조 (좌우 회전에 사용)

    [HideInInspector] public float xRotation = 0f;  // 카메라 위아래 회전 각도 저장, 인스펙터에선 안보임

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 마우스 커서 화면 중앙에 고정
        Cursor.visible = false;                     // 마우스 커서 숨기기
    }

    void Update()
    {
        // 마우스 움직임 입력값 받아오기 (X, Y 방향)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;               // 마우스 Y값 반영해 카메라 위아래 회전 각도 변경 (마우스 위로 움직이면 시선 아래로 내려감)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 위아래 회전 제한 (90도 이상 넘어가지 않도록)

        // 카메라(자신) 위아래 회전 적용 (localRotation)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 플레이어 몸통 좌우 회전 (Y축 기준)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
