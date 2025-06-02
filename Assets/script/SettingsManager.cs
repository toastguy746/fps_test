using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("Mouse Look Script")]
    public MouseLook mouseLookScript;

    void Start()
    {
        // 로드
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 100f);

        volumeSlider.value = savedVolume;
        sensitivitySlider.value = savedSensitivity;

        ApplyVolume(savedVolume);
        ApplySensitivity(savedSensitivity);

        // 이벤트 연결
        volumeSlider.onValueChanged.AddListener(ApplyVolume);
        sensitivitySlider.onValueChanged.AddListener(ApplySensitivity);
    }

    public void ApplyVolume(float value)
    {
        // AudioMixer는 dB 단위이므로 변환 필요
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void ApplySensitivity(float value)
    {
        if (mouseLookScript != null)
        {
            mouseLookScript.mouseSensitivity = value;
            PlayerPrefs.SetFloat("MouseSensitivity", value);
        }
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
