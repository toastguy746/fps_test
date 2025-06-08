using UnityEngine;
using TMPro;

public class AttachHoverEffectToAllText : MonoBehaviour
{
    public string targetTag = "Text";  // 효과를 붙일 대상 태그 이름

    void Start()
    {
        // 태그가 targetTag인 모든 게임오브젝트 찾기
        GameObject[] textObjects = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in textObjects)
        {
            // TextMeshProUGUI 컴포넌트가 없으면 경고 출력 후 건너뜀
            if (obj.GetComponent<TextMeshProUGUI>() == null)
            {
                Debug.LogWarning($"{obj.name}에는 TextMeshProUGUI가 없습니다. 효과를 적용하지 않습니다.");
                continue;
            }

            // TextHoverManager 컴포넌트가 없으면 새로 추가
            if (obj.GetComponent<TextHoverManager>() == null)
            {
                obj.AddComponent<TextHoverManager>();
            }
        }
    }
}
