using UnityEngine;
using System.Collections;
using TMPro;  // TMP용 네임스페이스 추가

public class DummyHealth : MonoBehaviour
{
    public int maxHealth = 200;
    public int health;

    [Header("체력 UI")]
    public TextMeshProUGUI healthText;  // TMP UI 텍스트

    private Transform mainCamera;

    void Start()
    {
        health = maxHealth;
        UpdateHealthUI();

        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        if (healthText != null && mainCamera != null)
        {
            // TMP UI 텍스트는 Canvas 렌더링 방식이라 월드 공간이면 이렇게 회전해야 함
            healthText.transform.rotation = Quaternion.LookRotation(healthText.transform.position - mainCamera.position);
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

        if (healthText != null)
            healthText.enabled = false;

        yield return new WaitForSeconds(3f);

        health = maxHealth;
        UpdateHealthUI();

        if (col != null) col.enabled = true;
        if (rend != null) rend.enabled = true;
        foreach (Renderer r in childRenderers)
            r.enabled = true;

        if (healthText != null)
            healthText.enabled = true;

        Debug.Log("더미 재생성 완료!");
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = health + " / " + maxHealth;
    }
}
