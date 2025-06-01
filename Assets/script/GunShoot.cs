using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GunShoot : MonoBehaviour
{
    [Header("총기 설정")]
    public float range = 100f;
    public float fireRate = 0.1f;

    [Header("탄창 설정")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 0.5f;
    private bool isReloading = false;

    [Header("이펙트")]
    public Light muzzleFlashLight;

    [Header("사운드")]
    public AudioClip gunshotSound;
    public AudioClip reloadClip;
    public AudioClip hitSound; // ✅ 추가
    private AudioSource audioSource;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public Image hitImage; // ✅ 추가
    public float hitImageDuration = 0.1f; // ✅ 추가

    [Header("카메라")]
    public Camera mainCamera;

    [Header("조준 컨트롤러")]
    public AimController aimController;

    private bool canShoot = true;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (muzzleFlashLight != null)
            muzzleFlashLight.enabled = false;

        if (hitImage != null)
            hitImage.enabled = false; // ✅ 이미지 비활성화로 시작

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && canShoot)
        {
            Shoot();
            StartCoroutine(ShootCooldown());
        }
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    void Shoot()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;
        UpdateAmmoUI();

        if (gunshotSound != null && audioSource != null)
            audioSource.PlayOneShot(gunshotSound);

        if (mainCamera == null)
        {
            Debug.LogError("GunShoot: mainCamera가 할당되지 않았습니다.");
            return;
        }

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 0.1f);

        if (Physics.Raycast(ray, out hit, range))
        {
            DummyHealth dummy = hit.collider.GetComponentInParent<DummyHealth>();
            if (dummy != null)
            {
                string hitPartTag = hit.collider.tag;
                dummy.OnHit(hitPartTag);

                PlayHitSound(); // ✅ 적중 효과음
                StartCoroutine(ShowHitImage()); // ✅ 이미지 표시
            }
        }

        if (muzzleFlashLight != null)
            StartCoroutine(MuzzleFlashLightEffect());
    }

    IEnumerator MuzzleFlashLightEffect()
    {
        muzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleFlashLight.enabled = false;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        canShoot = false;

        if (aimController != null)
            aimController.SetCanAim(false);

        if (reloadClip != null && audioSource != null)
            audioSource.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        isReloading = false;
        canShoot = true;

        if (aimController != null)
            aimController.SetCanAim(true);
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    // ✅ 적중 효과음
    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);
    }

    // ✅ 이미지 잠깐 표시
    IEnumerator ShowHitImage()
    {
        if (hitImage == null) yield break;

        hitImage.enabled = true;
        yield return new WaitForSeconds(hitImageDuration);
        hitImage.enabled = false;
    }
}
