using UnityEngine;
using TMPro;

public class AttachHoverEffectToAllText : MonoBehaviour
{
    public string targetTag = "Text";

    void Start()
    {
        GameObject[] textObjects = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in textObjects)
        {
            if (obj.GetComponent<TextMeshProUGUI>() == null)
            {
                Debug.LogWarning($"{obj.name}에는 TextMeshProUGUI가 없습니다. 효과를 적용하지 않습니다.");
                continue;
            }

            if (obj.GetComponent<TextHoverManager>() == null)
            {
                obj.AddComponent<TextHoverManager>();
            }
        }
    }
}
