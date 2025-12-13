using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float forwardSpeed = 5f;      // 자동 전진 속도

    [Header("충돌 설정")]
    public float invincibilityTime = 2f; // 피격 후 무적 시간
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    [Header("시각 효과")]
    public Renderer playerRenderer;      // 플레이어 렌더러 (깜빡임용)
    private float blinkTimer = 0f;
    private float blinkInterval = 0.1f;

    void Update()
    {
        // 자동으로 +Z 방향 이동
        transform.Translate(Vector3.forward * (forwardSpeed * Time.deltaTime), Space.World);

        // 무적 시간 처리
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            
            // 깜빡임 효과
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkInterval)
            {
                blinkTimer = 0f;
                if (playerRenderer)
                {
                    playerRenderer.enabled = !playerRenderer.enabled;
                }
            }

            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                if (playerRenderer)
                {
                    playerRenderer.enabled = true;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 무적 상태면 충돌 무시
        if (isInvincible) return;

        // 차량과 충돌 체크
        if (other.CompareTag("Car") || other.CompareTag("Bus"))
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage();
            
            // 무적 상태 활성화
            isInvincible = true;
            invincibilityTimer = invincibilityTime;
            blinkTimer = 0f;
            
            Debug.Log("[PlayerController] 차량과 충돌! 남은 체력: " + GameManager.Instance.health);
        }
    }

    // 게임 오버 시 이동 정지
    public void StopMovement()
    {
        forwardSpeed = 0f;
        enabled = false;
    }
}