using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Overlay Canvas - HUD (체력, 타이머)")]
    public TextMeshProUGUI healthText;   // 체력 표시 텍스트
    public TextMeshProUGUI timerText;    // 타이머 표시 텍스트

    [Header("World Space Canvas - 결과 패널")]
    public GameObject resultPanel;       // 게임 오버/클리어 공용 패널
    public Image panelBackground;        // 패널 배경 이미지 (색상 변경용)
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI resultDetailText;

    [Header("버튼")]
    public Button restartButton;         // 재시작 버튼
    public Button mainMenuButton;        // 메인 메뉴로 돌아가기 버튼

    [Header("패널 위치 설정")]
    public Transform player;             // 플레이어 Transform
    public Vector3 panelOffset = new Vector3(0f, 1.5f, 3f);  // 플레이어 기준 오프셋

    [Header("패널 색상 설정")]
    public Color gameOverColor = new Color(0.8f, 0.2f, 0.2f, 0.9f);   // 게임 오버 색상 (빨강)
    public Color gameClearColor = new Color(0.2f, 0.8f, 0.2f, 0.9f);  // 게임 클리어 색상 (초록)

    void Start()
    {
        // 패널 초기 비활성화
        if (resultPanel)
            resultPanel.SetActive(false);

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
        if (resultPanel)
        {
            // 패널 활성화
            resultPanel.SetActive(true);
            
            // 플레이어 위치 + 오프셋으로 이동
            PositionPanel();
            
            // 게임 오버 색상으로 변경
            SetPanelColor(gameOverColor);
            
            // 텍스트 설정
            if (resultTitleText)
                resultTitleText.text = "게임 오버!";

            if (resultDetailText)
                resultDetailText.text = "사장님에게 도망가지 못 했습니다.";
        }
    }

    public void ShowGameClear()
    {
        if (resultPanel)
        {
            // 패널 활성화
            resultPanel.SetActive(true);
            
            // 플레이어 위치 + 오프셋으로 이동
            PositionPanel();
            
            // 게임 클리어 색상으로 변경
            SetPanelColor(gameClearColor);
            
            // 텍스트 설정
            if (resultTitleText)
                resultTitleText.text = "게임 클리어!";
            
            if (resultDetailText)
                resultDetailText.text = "사장님한테 도망치기 성공했습니다.";
        }
    }

    // 패널 위치를 플레이어 + 오프셋으로 설정
    void PositionPanel()
    {
        if (!player || !resultPanel) return;

        resultPanel.transform.position = player.position + panelOffset;
    }

    // 패널 배경 색상 변경
    void SetPanelColor(Color color)
    {
        if (panelBackground)
        {
            panelBackground.color = color;
        }
    }
}