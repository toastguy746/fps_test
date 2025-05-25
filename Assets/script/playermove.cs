using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;                 // 기본 이동 속도
    public float sprintMultiplier = 2f;          // 달리기 속도 배수
    public float jumpVelocity = 8f;               // 점프 속도(초기 상승 속도)
    public float gravity = -20f;                  // 중력 가속도 (음수)

    [Header("Ground 체크")]
    public Transform groundCheck;                 // 땅 체크 위치 (발 근처)
    public float groundDistance = 0.3f;           // 땅 감지 반경
    public LayerMask groundMask;                   // 땅 레이어만 감지

    [Header("회전 기준")]
    public Transform playerBody;                  // 플레이어 몸통 (이동 방향 기준)

    [Header("발소리 설정")]
    public AudioClip footaudio;                   // 발걸음 소리 클립
    public AudioSource footAudioSource;          // 오디오 소스
    public float stepInterval = 0.5f;             // 발걸음 소리 간격

    private CharacterController controller;      // 캐릭터 컨트롤러 컴포넌트
    private Vector3 velocity;                     // 플레이어 현재 속도 (중력 포함)
    private bool isGrounded;                      // 바닥에 닿았는지 여부
    private float stepTimer;                      // 발소리 재생 타이머

    void Start()
    {
        controller = GetComponent<CharacterController>(); // 캐릭터컨트롤러 초기화
        stepTimer = stepInterval;                         // 타이머 초기화

        if (footAudioSource != null)
        {
            footAudioSource.loop = false;                 // 발소리 루프 끔
            footAudioSource.playOnAwake = false;          // 시작시 재생 끔
        }
    }

    void Update()
    {
        // 바닥과 닿았는지 체크 (groundCheck 위치 기준 구 형태)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 바닥에 닿고 내려가는 중이면 약간 떨어진 위치 고정 (중력 완화용)
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // 입력받은 좌우, 앞뒤 이동량
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // 입력 방향 벡터 계산 (플레이어 몸통 기준)
        Vector3 move = playerBody.right * x + playerBody.forward * z;
        move.Normalize();  // 정규화(대각선 이동 시 속도 일정 유지)

        // 달리기 상태에 따라 속도 결정
        float currentSpeed = moveSpeed;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        if (isSprinting)
            currentSpeed *= sprintMultiplier;

        // 캐릭터 이동 (이동 벡터 * 속도 * 시간)
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 점프 입력 처리 (땅에 닿아있을 때만 점프 가능)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            velocity.y = jumpVelocity;

        // 중력 적용
        if (!isGrounded)
        {
            float fallMultiplier = 2.5f; // 떨어질 때 중력 가속도 증가
            float riseMultiplier = 2f;   // 올라갈 때 중력 가속도 증가

            if (velocity.y < 0)          // 내려갈 때
                velocity.y += gravity * fallMultiplier * Time.deltaTime;
            else                        // 올라갈 때
                velocity.y += gravity * riseMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime; // 착지 직후 안정화용 중력 미세적용
        }

        // 중력 적용된 속도로 이동
        controller.Move(velocity * Time.deltaTime);

        // 발소리 재생 처리
        HandleFootsteps(isGrounded, isSprinting, x, z);
    }

    void HandleFootsteps(bool grounded, bool isSprinting, float xInput, float zInput)
    {
        // 움직이고 있는지 체크 (입력값이 거의 0 이상인지)
        bool isMoving = Mathf.Abs(xInput) > 0.1f || Mathf.Abs(zInput) > 0.1f;

        if (grounded && isMoving && footaudio != null && footAudioSource != null)
        {
            // 달릴 때는 발소리 간격을 반으로 줄임
            float interval = isSprinting ? stepInterval / 2f : stepInterval;

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                footAudioSource.PlayOneShot(footaudio);  // 발소리 재생
                stepTimer = interval;                     // 타이머 리셋
            }
        }
        else
        {
            stepTimer = stepInterval; // 안 움직이거나 공중이면 타이머 초기화
        }
    }
}
