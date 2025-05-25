using UnityEngine;
using System.Collections;

public class GunShoot : MonoBehaviour
{
    [Header("총기 설정")]
    public float range = 100f;          // 총알 최대 사거리
    public float fireRate = 0.1f;       // 총 발사 딜레이 (초)

    [Header("이펙트")]
    public Transform firePoint;          // 총알 발사 위치 (사용은 안 됨 현재)
    public GameObject muzzleFlash;       // 총구 화염 효과 오브젝트
    public Light muzzleFlashLight;       // 총구 화염을 위한 라이트 컴포넌트

    [Header("사운드")]
    public AudioClip gunshotSound;       // 총소리 오디오 클립

    private bool canShoot = true;        // 쏠 수 있는 상태인지 여부
    private AudioSource audioSource;     // 사운드 재생용 컴포넌트

    void Start()
    {
        // 오디오소스 컴포넌트가 없으면 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 총구 라이트는 처음에 꺼놓음
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = false;
        }

        audioSource.playOnAwake = false; // 시작할 때 소리 자동 재생 안함
    }

    void Update()
    {
        // 마우스 왼쪽 클릭 중이고, 쏠 수 있을 때 총 쏘기
        if (Input.GetButton("Fire1") && canShoot)
        {
            Shoot();
            StartCoroutine(ShootCooldown()); // 재발사 쿨타임 시작
        }
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;                  // 쏘지 못하게 막음
        yield return new WaitForSeconds(fireRate);  // fireRate만큼 대기
        canShoot = true;                   // 다시 쏠 수 있게 허용
    }

    void Shoot()
    {
        Debug.Log("총 발사");             // 디버그용 출력

        // 총소리 재생
        if (gunshotSound != null && audioSource != null)
            audioSource.PlayOneShot(gunshotSound);

        // 카메라 정중앙에서 레이 쏘기
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // 레이 시각화 (빨간색, 0.1초간 보임)
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 0.1f);

        // 레이가 무언가 맞았을 때
        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log($"맞음: {hit.collider.name}");  // 맞은 콜라이더 이름 출력

            // 맞은 콜라이더의 부모 중 DummyHealth 컴포넌트 찾기
            DummyHealth dummy = hit.collider.GetComponentInParent<DummyHealth>();
            if (dummy != null)
            {
                // 맞은 부위 태그 (Head, Body 등) 넘겨서 데미지 처리 요청
                string hitPartTag = hit.collider.tag;  
                dummy.OnHit(hitPartTag);
            }
        }

        // 총구 화염 이펙트 코루틴 실행
        StartCoroutine(MuzzleFlashEffect());
    }

    IEnumerator MuzzleFlashEffect()
    {
        // 총구 화염 오브젝트 켜기
        if (muzzleFlash != null) muzzleFlash.SetActive(true);
        // 총구 라이트 켜기
        if (muzzleFlashLight != null) muzzleFlashLight.enabled = true;

        // 0.05초 대기 (짧게 켜짐)
        yield return new WaitForSeconds(0.05f);

        // 총구 화염 오브젝트 끄기
        if (muzzleFlash != null) muzzleFlash.SetActive(false);
        // 총구 라이트 끄기
        if (muzzleFlashLight != null) muzzleFlashLight.enabled = false;
    }
}
