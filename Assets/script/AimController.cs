using UnityEngine;

public class AimController : MonoBehaviour
{
    private bool canAim = true;
    [Header("카메라")]
    public Camera mainCamera;

    [Header("FOV 설정")]
    public float normalFOV = 60f;
    public float aimFOV = 30f;
    public float zoomSpeed = 10f;


    [HideInInspector] public bool isAiming;
    public void SetCanAim(bool value)
    {
        canAim = value;
        if (!value) isAiming = false; // 강제로 조준 해제
    }
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (canAim)
            isAiming = Input.GetMouseButton(1); // 우클릭


        float targetFOV = isAiming ? aimFOV : normalFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }


    public bool IsAiming()
    {
        return isAiming;
    }
}
