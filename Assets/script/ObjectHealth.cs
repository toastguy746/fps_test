using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ObjectHealth : MonoBehaviour
{
    // 플레이어 맞음?
    public bool isPlayer;

    // 최대 체력하고 현재체력 확인
    public int maxHealth = 200;
    private int currentHealth;

    // 체력바 UI
    public Slider healthBarSlider;
    public Image fillImage;

    // 플레이어한테 쓸 체력 텍스트
    public TextMeshProUGUI healthText;

    // 플레이어용 피격 사운드
    public AudioClip hitSound;
    private AudioSource audioSource;

    // 메인 카메라, 렌더러, 콜라이더, 시작 위치 설정하기(걍 스폰 위치 알려줄 엠티 만들어주고 스폰 장소라고 할당하면 될듯?)
    private Transform mainCamera;
    private Renderer[] renderers;
    private Collider col;
    private Vector3 originalPosition;

    // 플레이어 이동하는거랑 마우스 시야이동하는 스크립트 참고하기
    private PlayerMovement playerMovement;
    private MouseLook mouseLook;

    // 더미 총알 발사 스크립트
    private DummyShooter dummyShooter;

    void Start()
    {
        currentHealth = maxHealth;

        mainCamera = Camera.main.transform;
        renderers = GetComponentsInChildren<Renderer>();
        col = GetComponent<Collider>();
        originalPosition = transform.position;

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }

        if (isPlayer)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            UpdateHealthText();

            playerMovement = GetComponent<PlayerMovement>();
            mouseLook = GetComponentInChildren<MouseLook>();
        }
        else
        {
            dummyShooter = GetComponent<DummyShooter>();
        }
    }

    void Update()
    {
        // 더미 체력바가 항상 카메라를 바라보게
        if (healthBarSlider != null && mainCamera != null && !isPlayer)
        {
            healthBarSlider.transform.rotation =
                Quaternion.LookRotation(healthBarSlider.transform.position - mainCamera.position);
        }
    }

    // 머리 맞으면 100 몸 맞으면 50 이런식으로 하면 될듯 (걍 몸의 2배?)
    public void OnHit(string hitPartTag)
    {
        int damage = (hitPartTag == "Head") ? 100 :
                     (hitPartTag == "Body") ? 50 : 0;

        TakeDamage(damage);
    }

    // 데미지 받으면 그 만큼 체력 감소하게
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // currentHealth가 0 미만이면 0으로 고정
        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log($"[{(isPlayer ? "플레이어" : "더미")}] 데미지: {damage}, 남은 체력: {currentHealth}");

        UpdateHealthUI();

        if (isPlayer)
        {
            PlayHitSound();
            UpdateHealthText();
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(isPlayer ? PlayerDieAndRespawn() : DummyDieAndRespawn());
        }
    }

    // 플레이어 죽는거랑 리스폰
    IEnumerator PlayerDieAndRespawn()
    {
        Debug.Log("플레이어 사망!");

        // 이동, 시야 컨트롤 잠금 해제 부분 주석 처리
        // if (playerMovement != null) playerMovement.enabled = false;
        // if (mouseLook != null) mouseLook.enabled = false;

        SetRenderersVisible(false);
        if (col != null) col.enabled = false;
        if (healthBarSlider != null) healthBarSlider.gameObject.SetActive(false);
        if (healthText != null) healthText.gameObject.SetActive(false);

        yield return new WaitForSeconds(4f);

        Respawn();

        // if (playerMovement != null) playerMovement.enabled = true;
        // if (mouseLook != null) mouseLook.enabled = true;
    }



    // 더미 사망, 리스폰
    IEnumerator DummyDieAndRespawn()
    {
        Debug.Log("더미 사망!");

        // 총알 발사 멈추기
        if (dummyShooter != null)
            dummyShooter.enabled = false;

        SetRenderersVisible(false);
        if (col != null) col.enabled = false;
        if (healthBarSlider != null) healthBarSlider.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        Respawn();

        if (dummyShooter != null)
            dummyShooter.enabled = true;
    }

    // 리스폰
    void Respawn()
    {
        transform.position = originalPosition;
        currentHealth = maxHealth;

        // [추가] 플레이어 리스폰 시 회전 초기화 처리
        if (isPlayer)
        {
            transform.rotation = Quaternion.identity;

            if (mouseLook != null)
            {
                mouseLook.ResetRotation();
            }
        }

        UpdateHealthUI();
        UpdateHealthText();

        SetRenderersVisible(true);
        // SetRenderersAlpha(1f); // 투명도 복구 제거

        if (col != null) col.enabled = true;
        if (healthBarSlider != null) healthBarSlider.gameObject.SetActive(true);
        if (healthText != null) healthText.gameObject.SetActive(true);

        Debug.Log($"{(isPlayer ? "플레이어" : "더미")} 리스폰 완료!");
    }


    // 렌더러 켜기/끄기
    void SetRenderersVisible(bool visible)
    {
        foreach (var r in renderers)
            r.enabled = visible;
    }

    // 렌더러 투명도 설정
    void SetRenderersAlpha(float alpha)
    {
        // foreach (var r in renderers)
        // {
        //     foreach (var mat in r.materials)
        //     {
        //         Material matInstance = mat;
        //         Color color = matInstance.color;
        //         color.a = alpha;
        //         matInstance.color = color;

        //         matInstance.SetFloat("_Mode", 2);
        //         matInstance.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //         matInstance.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //         matInstance.SetInt("_ZWrite", 0);
        //         matInstance.DisableKeyword("_ALPHATEST_ON");
        //         matInstance.EnableKeyword("_ALPHABLEND_ON");
        //         matInstance.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //         matInstance.renderQueue = 3000;
        //     }
        // }
    }


    // 체력바 UI 갱신
    void UpdateHealthUI()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;

            if (fillImage != null)
            {
                Color c = fillImage.color;
                c.a = 1f;
                fillImage.color = c;
            }
        }
    }

    // 체력 텍스트 갱신
    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    // 피격 사운드 재생
    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
}
