using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("게임 설정")]
    public int maxHealth = 4;
    public float gameDuration = 180f;
    
    [Header("씬 이름")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GamePlay";
    
    [Header("상태")]
    public int health;
    public float timeRemaining;
    public bool isGameOver = false;
    public bool isGameClear = false;

    [Header("참조")]
    public PlayerController player;
    public UIManager uiManager;

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 초기화
        health = maxHealth;
        timeRemaining = gameDuration;
        isGameOver = false;
        isGameClear = false;

        // UI 업데이트
        if (uiManager)
        {
            uiManager.UpdateHealth(health);
            uiManager.UpdateTimer(timeRemaining);
        }
    }

    void Update()
    {
        // 게임이 끝났으면 더이상 진행 안함
        if (isGameOver || isGameClear) return;

        // 타이머 감소
        timeRemaining -= Time.deltaTime;
        
        if (uiManager)
            uiManager.UpdateTimer(timeRemaining);

        // 시간이 다 되면 게임 클리어!
        if (timeRemaining <= 0f)
            GameClear();
    }

    public void TakeDamage()
    {
        if (isGameOver || isGameClear) return;

        health--;
        
        if (uiManager)
            uiManager.UpdateHealth(health);

        if (health <= 0)
            GameOver();
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("[GameManager] 게임 오버!");

        // 플레이어 이동 정지
        if (player)
            player.StopMovement();

        // UI 표시
        if (uiManager)
            uiManager.ShowGameOver();
    }

    void GameClear()
    {
        isGameClear = true;
        Debug.Log("[GameManager] 게임 클리어!");

        // 플레이어 이동 정지
        if (player)
            player.StopMovement();

        // UI 표시
        if (uiManager)
            uiManager.ShowGameClear();
    }

    public void RestartGame()
    {
        // 현재 게임 씬 재시작
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMainMenu()
    {
        // 메인 메뉴로 이동
        SceneManager.LoadScene(mainMenuSceneName);
    }
}