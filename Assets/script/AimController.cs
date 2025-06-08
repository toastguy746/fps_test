using UnityEngine;

public class AimController : MonoBehaviour
{
    // 조준 가능 여부 제어 플래그
    private bool canAim = true;

    [Header("카메라")]
    public Camera mainCamera;

    [Header("FOV 설정")]
    public float normalFOV = 60f;    // 기본 시야각
    public float aimFOV = 30f;       // 조준 시 시야각
    public float zoomSpeed = 10f;    // FOV 전환 속도

    [HideInInspector]
    public bool isAiming;  // 현재 조준 상태

    // --- 애니메이션 관련 변수 ---
    [Header("애니메이션")]
    public Animator ak47Animator;    // AK-47 무기 애니메이터

    // Animator 내 조준 상태를 나타내는 Bool 파라미터 이름
    [SerializeField]
    private string aimParamName = "aimis";

    /// <summary>
    /// 조준 가능 여부를 외부에서 설정
    /// false일 경우 강제로 조준 해제 처리 및 애니메이터 파라미터도 false로 변경
    /// </summary>
    public void SetCanAim(bool value)
    {
        canAim = value;
        if (!value)
        {
            isAiming = false;

            if (ak47Animator != null)
            {
                ak47Animator.SetBool(aimParamName, false);
            }
        }
    }

    void Start()
    {
        // mainCamera 미할당 시 자동으로 메인 카메라 할당
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Animator가 할당되지 않았다면 컴포넌트에서 찾아서 할당 시도
        if (ak47Animator == null)
        {
            ak47Animator = GetComponent<Animator>();

            if (ak47Animator == null)
            {
                Debug.LogError("AimController: AK-47 Animator not found! Please assign it in the Inspector or ensure it's on this GameObject or its children.");
            }
        }
    }

    void Update()
    {
        // 이전 프레임의 조준 상태 저장
        bool previousIsAiming = isAiming;

        // 조준 가능할 때 우클릭 시 조준 상태 갱신
        if (canAim)
            isAiming = Input.GetMouseButton(1);

        // 목표 FOV 값 계산 (조준 시와 아닐 때 다름)
        float targetFOV = isAiming ? aimFOV : normalFOV;

        // 카메라 시야각을 부드럽게 변경
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);

        // 조준 상태 변경 시에만 애니메이터 파라미터 업데이트
        if (isAiming != previousIsAiming)
        {
            if (ak47Animator != null)
            {
                ak47Animator.SetBool(aimParamName, isAiming);
            }
        }
    }

    /// 지금 조준중임?
    public bool IsAiming()
    {
        return isAiming;
    }
}
