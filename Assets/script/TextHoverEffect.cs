using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

// TextMeshProUGUI 텍스트에 마우스 호버 효과를 주는 매니저 클래스
// 마우스가 텍스트 위에 올라가면 크기가 커지고 소리가 난다
public class TextHoverManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;          // 원래 크기 저장용 변수
    public float scaleMultiplier = 1.1f;    // 호버 시 크기 배율

    private TextMeshProUGUI tmp;            // 이 오브젝트의 TextMeshProUGUI 컴포넌트 참조
    private AudioSource audioSource;        // 사운드 재생용 AudioSource 컴포넌트

    public AudioClip hoverSound;            // 인스펙터에서 할당하는 호버 사운드

    void Start()
    {
        // TextMeshProUGUI 컴포넌트 가져오기
        tmp = GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            // 컴포넌트 없으면 경고 로그 출력 후 이 스크립트 비활성화
            Debug.LogWarning($"{gameObject.name} - TextHoverManager는 TextMeshProUGUI가 필요합니다.");
            enabled = false;
            return;
        }

        // 오브젝트 원래 크기 저장
        originalScale = transform.localScale;

        // AudioSource 컴포넌트 추가 및 초기 설정
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;   // 오브젝트 시작 시 소리 재생하지 않도록 설정
    }

    // 마우스가 텍스트 위로 진입했을 때 호출되는 함수
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 크기 키우기 (원래 크기에 배율 곱함)
        transform.localScale = originalScale * scaleMultiplier;

        // 호버 사운드가 설정되어 있으면 재생
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // 마우스가 텍스트에서 벗어났을 때 호출되는 함수
    public void OnPointerExit(PointerEventData eventData)
    {
        // 크기 원래대로 복원
        transform.localScale = originalScale;
    }
}
