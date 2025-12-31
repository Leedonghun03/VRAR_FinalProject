using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("씬 설정")]
    public string gameSceneName = "GamePlay";  // 게임 플레이 씬 이름

    [Header("UI 참조")]
    public Button playButton;       // 게임 시작 버튼
    public Button quitButton;       // 게임 종료 버튼

    void Start()
    {
        // 버튼 이벤트 연결
        if (playButton)
            playButton.onClick.AddListener(StartGame);

        if (quitButton)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        Debug.Log("[MainMenu] 게임 시작!");
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenu] 게임 종료!");
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}