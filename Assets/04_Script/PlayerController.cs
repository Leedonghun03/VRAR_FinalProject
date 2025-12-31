using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float forwardSpeed = 5f;      // 자동 전진 속도

    [Header("이동 방식 선택")]
    [Tooltip("0: 카메라 방향으로 이동 (VR용), 1: Z축 방향으로 이동 (기존 방식)")]
    public int moveMode = 0;  // 0 = 카메라 방향, 1 = Z축 방향

    [Header("충돌 설정")]
    public float invincibilityTime = 2f; // 피격 후 무적 시간
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    [Header("시각 효과")]
    public Renderer playerRenderer;      // 플레이어 렌더러 (깜빡임용)
    private float blinkTimer = 0f;
    private float blinkInterval = 0.1f;

    // 컴포넌트 참조
    private CharacterController characterController;
    private Camera playerCamera;

    void Start()
    {
        // CharacterController 가져오기 (없으면 추가)
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            Debug.Log("[PlayerController] CharacterController 자동 추가됨");
        }

        // 메인 카메라 참조
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("[PlayerController] 메인 카메라를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        // 이동 처리
        if (moveMode == 0)
        {
            MoveWithCamera();  // 카메라 방향으로 이동 (VR용)
        }
        else
        {
            MoveForward();     // 기존 방식 (Z축 방향)
        }

        // 무적 시간 처리
        HandleInvincibility();
    }

    // 카메라가 바라보는 방향으로 이동 (ForceMove 방식)
    void MoveWithCamera()
    {
        if (playerCamera == null || characterController == null) return;

        // 현재 바라보고 있는 카메라의 방향이 플레이어가 움직이는 방향
        Vector3 forwardDir = playerCamera.transform.forward;

        // 해당 방향의 높이 값을 0으로 주어서 움직일때 높이와 상관 없는 방향성 제시
        forwardDir.y = 0;
        forwardDir.Normalize();  // 정규화하여 일정한 속도 유지

        // CharacterController를 SimpleMove를 통해서 이동
        characterController.SimpleMove(forwardDir * forwardSpeed);
    }

    // 기존 방식: Z축 방향으로 이동
    void MoveForward()
    {
        transform.Translate(Vector3.forward * (forwardSpeed * Time.deltaTime), Space.World);
    }

    // 무적 시간 처리
    void HandleInvincibility()
    {
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