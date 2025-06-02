using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;
    public float jumpVelocity = 8f;
    public float gravity = -20f;

    [Header("Ground 체크")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("회전 기준")]
    public Transform playerBody;

    [Header("발소리 설정")]
    public AudioClip footaudio;
    public AudioSource footAudioSource;
    public float stepInterval = 0.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float stepTimer;

    // ✅ 점프 입력 버퍼 관련 변수
    private float jumpBufferTime = 0.2f; // 입력 유지 시간
    private float jumpBufferCounter = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stepTimer = stepInterval;

        if (footAudioSource != null)
        {
            footAudioSource.loop = false;
            footAudioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // --- Ground 체크 ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- 이동 입력 처리 ---
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = playerBody.right * x + playerBody.forward * z;
        move.Normalize();

        float currentSpeed = moveSpeed;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        if (isSprinting)
            currentSpeed *= sprintMultiplier;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- ✅ 점프 입력 버퍼 처리 ---
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;

        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0 && isGrounded)
        {
            velocity.y = jumpVelocity;
            jumpBufferCounter = 0f;
        }

        // --- 중력 처리 ---
        if (!isGrounded)
        {
            float fallMultiplier = 2.5f;
            float riseMultiplier = 2f;

            if (velocity.y < 0)
                velocity.y += gravity * fallMultiplier * Time.deltaTime;
            else
                velocity.y += gravity * riseMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);

        // --- 발소리 처리 ---
        HandleFootsteps(isGrounded, isSprinting, x, z);
    }

    void HandleFootsteps(bool grounded, bool isSprinting, float xInput, float zInput)
    {
        bool isMoving = Mathf.Abs(xInput) > 0.1f || Mathf.Abs(zInput) > 0.1f;

        if (grounded && isMoving && footaudio != null && footAudioSource != null)
        {
            float interval = isSprinting ? stepInterval / 2f : stepInterval;
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                footAudioSource.PlayOneShot(footaudio);
                stepTimer = interval;
            }
        }
        else
        {
            stepTimer = stepInterval;
        }
    }
}
