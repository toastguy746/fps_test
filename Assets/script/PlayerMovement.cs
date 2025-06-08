using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;                // 기본 이동 속도
    public float sprintMultiplier = 2f;         // 달리기 속도 배수
    public float jumpVelocity = 8f;              // 점프 초기 속도
    public float gravity = -20f;                 // 중력 가속도

    [Header("Ground 체크")]
    public Transform groundCheck;                // 땅 체크 위치 (발 아래)
    public float groundDistance = 0.3f;          // 땅 체크 반경
    public LayerMask groundMask;                 // 땅으로 인식할 레이어

    [Header("회전 기준")]
    public Transform playerBody;                 // 플레이어 방향 기준(주로 카메라의 y축 회전)

    [Header("발소리 설정")]
    public AudioClip footaudio;                  // 발소리 오디오 클립
    public AudioSource footAudioSource;          // 발소리 재생용 AudioSource
    public float stepInterval = 0.2f;             // 한 걸음 간격 (초 단위)

    private CharacterController controller;      // 캐릭터 컨트롤러 컴포넌트
    private Vector3 velocity;                     // 현재 속도 (특히 y축 중력/점프용)
    private bool isGrounded;                      // 땅에 닿았는지 여부
    private float stepTimer;                      // 발소리 타이머

    // ✅ 점프 입력 버퍼 변수
    private float jumpBufferTime = 0.2f;         // 점프 입력 유효 시간 (버퍼)
    private float jumpBufferCounter = 0f;        // 현재 점프 입력 버퍼 카운터

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stepTimer = stepInterval;

        if (footAudioSource != null)
        {
            footAudioSource.loop = false;          // 발소리 반복 재생 금지
            footAudioSource.playOnAwake = false;   // 시작 시 자동 재생 금지
        }
    }

    void Update()
    {
        if (!controller.enabled) return;          // 컨트롤러 비활성 시 처리 중단

        // --- 땅 체크 ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;                      // 땅에 붙어있을 때 속도 리셋 (약간 내려가게 하여 땅에 밀착)

        // --- 이동 입력 처리 ---
        float x = Input.GetAxisRaw("Horizontal"); // 좌우 입력 (-1 ~ 1)
        float z = Input.GetAxisRaw("Vertical");   // 앞뒤 입력 (-1 ~ 1)

        // 이동 방향 계산 (플레이어 몸체 기준)
        Vector3 move = playerBody.right * x + playerBody.forward * z;
        move.Normalize();                          // 대각선 이동 시 속도 보정

        float currentSpeed = moveSpeed;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        if (isSprinting)
            currentSpeed *= sprintMultiplier;      // 달리기 시 속도 배수 적용

        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- 점프 입력 버퍼 처리 ---
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;   // 점프 입력 들어오면 버퍼 초기화

        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;  // 버퍼 시간 감소

        if (jumpBufferCounter > 0 && isGrounded)
        {
            velocity.y = jumpVelocity;              // 땅에 닿아있으면 점프 실행
            jumpBufferCounter = 0f;                 // 버퍼 초기화
        }

        // --- 중력 처리 ---
        if (!isGrounded)
        {
            float fallMultiplier = 2.5f;            // 낙하 시 중력 가속도 배수
            float riseMultiplier = 2f;              // 상승 시 중력 가속도 배수 (점프 끊기용)

            if (velocity.y < 0)
                velocity.y += gravity * fallMultiplier * Time.deltaTime;  // 떨어질 때 빠르게 떨어짐
            else
                velocity.y += gravity * riseMultiplier * Time.deltaTime;  // 올라갈 때 중력 적용 (점프 끊김 효과)
        }
        else
        {
            velocity.y += gravity * Time.deltaTime; // 땅에 붙어있어도 약간 중력 적용
        }

        controller.Move(velocity * Time.deltaTime);

        // --- 발소리 처리 ---
        HandleFootsteps(isGrounded, isSprinting, x, z);
    }

    // 발소리 재생 처리
    void HandleFootsteps(bool grounded, bool isSprinting, float xInput, float zInput)
    {
        bool isMoving = Mathf.Abs(xInput) > 0.1f || Mathf.Abs(zInput) > 0.1f;

        if (grounded && isMoving && footaudio != null && footAudioSource != null)
        {
            float interval = isSprinting ? stepInterval / 2f : stepInterval;  // 달리기 시 발걸음 간격 단축
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                footAudioSource.PlayOneShot(footaudio);  // 발소리 재생
                stepTimer = interval;                     // 타이머 초기화
            }
        }
        else
        {
            stepTimer = stepInterval;  // 움직이지 않거나 공중에 있으면 타이머 리셋
        }
    }
}
