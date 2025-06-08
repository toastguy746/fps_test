using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // 마우스 감도 설정
    public float mouseSensitivity = 150f;

    // 플레이어 몸체 오브젝트 (Y축 회전 적용 대상)
    public Transform playerBody;

    // X축 회전 누적값 (카메라의 상하 회전)
    [HideInInspector] public float xRotation = 0f;

    // 추가: 죽음 상태 플래그
    public bool isDead = false;

    void Start()
    {
        // 마우스 커서를 화면 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;

        // 마우스 커서 숨김
        Cursor.visible = false;
    }

    void Update()
    {
        // 죽었으면 입력 무시
        if (isDead) return;

        // 마우스 입력값 받기 (X축: 좌우, Y축: 상하)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 카메라 상하 회전 누적값 갱신 (마우스 위로 이동 시 아래를 바라보도록 음수 처리)
        xRotation -= mouseY;

        // 상하 회전 각도를 -90도~90도로 제한 (고개 꺾임 방지)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 카메라 상하 회전 적용
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 플레이어 몸체 좌우 회전 적용
        playerBody.Rotate(Vector3.up * mouseX);
    }   

    public void ResetRotation()
    {
        xRotation = 0f;  // 여기만 바꾸면 됨
        transform.localRotation = Quaternion.identity;
    }

}
