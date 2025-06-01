using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DummyHealth : MonoBehaviour
{
    public int maxHealth = 200;
    public int health;

    [Header("체력 UI")]
    public Slider healthBarSlider;  // 체력바용 슬라이더
    public Image fillImage;          // 슬라이더의 Fill 이미지 연결

    private Transform mainCamera;

    void Start()
    {
        health = maxHealth;

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = health;
        }

        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        if (healthBarSlider != null && mainCamera != null)
        {
            // 체력바가 월드 공간에 있을 때 카메라 쪽으로 회전
            healthBarSlider.transform.rotation = Quaternion.LookRotation(healthBarSlider.transform.position - mainCamera.position);
        }
    }

    public void OnHit(string hitPartTag)
    {
        int damage = 0;

        if (hitPartTag == "Head")
            damage = 100;
        else if (hitPartTag == "Body")
            damage = 50;

        TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"데미지: {damage}, 남은 체력: {health}");

        UpdateHealthUI();

        if (health <= 0)
            StartCoroutine(DieAndRespawn());
    }

    IEnumerator DieAndRespawn()
    {
        Debug.Log("더미 사망!");

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Renderer rend = GetComponent<Renderer>();
        if (rend != null) rend.enabled = false;

        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in childRenderers)
            r.enabled = false;

        if (healthBarSlider != null)
            healthBarSlider.gameObject.SetActive(false);

        yield return new WaitForSeconds(3f);

        health = maxHealth;
        UpdateHealthUI();

        if (col != null) col.enabled = true;
        if (rend != null) rend.enabled = true;
        foreach (Renderer r in childRenderers)
            r.enabled = true;

        if (healthBarSlider != null)
            healthBarSlider.gameObject.SetActive(true);

        Debug.Log("더미 재생성 완료!");
    }

    void UpdateHealthUI()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;  // 여기서 체력에 맞춰 슬라이더 값 세팅

            if (fillImage != null)
            {
                Color c = fillImage.color;
                c.a = 1f;  // 투명도는 1로 고정 (완전 불투명)
                fillImage.color = c;
            }
        }
    }

}
