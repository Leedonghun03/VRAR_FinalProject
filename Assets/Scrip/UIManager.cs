using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro 사용

public class UIManager : MonoBehaviour
{
    [Header("Overlay Canvas - HUD (체력, 타이머)")]
    public TextMeshProUGUI healthText;   // 체력 표시 텍스트
    public TextMeshProUGUI timerText;    // 타이머 표시 텍스트
    public Image[] healthIcons;          // 하트 이미지 배열 (옵션)

    [Header("World Space Canvas - 게임 오버/클리어 패널")]
    public GameObject gameOverPanel;     // World Space 패널
    public GameObject gameClearPanel;    // World Space 패널
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI gameClearText;

    [Header("버튼")]
    public Button restartButton;
    public Button mainMenuButton;        // 메인 메뉴로 돌아가기 버튼

    void Start()
    {
        // 패널 초기 비활성화
        if (gameOverPanel)
            gameOverPanel.SetActive(false);
        if (gameClearPanel)
            gameClearPanel.SetActive(false);

        // 버튼 이벤트 연결
        if (restartButton)
        {
            restartButton.onClick.AddListener(() => {
                if (GameManager.Instance != null)
                    GameManager.Instance.RestartGame();
            });
        }

        if (mainMenuButton)
        {
            mainMenuButton.onClick.AddListener(() => {
                if (GameManager.Instance)
                    GameManager.Instance.GoToMainMenu();
            });
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        // 텍스트 업데이트
        if (healthText)
        {
            healthText.text = "체력: " + currentHealth;
        }

        // 하트 이미지 업데이트 (옵션)
        if (healthIcons != null && healthIcons.Length > 0)
        {
            for (int i = 0; i < healthIcons.Length; i++)
            {
                if (healthIcons[i] != null)
                {
                    healthIcons[i].enabled = (i < currentHealth);
                }
            }
        }
    }

    public void UpdateTimer(float timeRemaining)
    {
        if (timerText)
        {
            // 시간을 분:초 형식으로 표시
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = string.Format("시간: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText)
        {
            gameOverText.text = "게임 오버!\n차량과의 충돌로 게임이 종료되었습니다.";
        }
    }

    public void ShowGameClear()
    {
        if (gameClearPanel)
        {
            gameClearPanel.SetActive(true);
        }

        if (gameClearText)
        {
            gameClearText.text = "게임 클리어!\n3분 생존에 성공했습니다!";
        }
    }
}