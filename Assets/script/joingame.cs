using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TextButton : MonoBehaviour
{
    public string sceneToLoad = "GameScene";

    public void OnClickText()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
