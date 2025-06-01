using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextHoverManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.1f;

    private TextMeshProUGUI tmp;
    private AudioSource audioSource;

    public AudioClip hoverSound;  // 인스펙터에서 할당

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogWarning($"{gameObject.name} - TextHoverManager는 TextMeshProUGUI가 필요합니다.");
            enabled = false;
            return;
        }

        originalScale = transform.localScale;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleMultiplier;

        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}
