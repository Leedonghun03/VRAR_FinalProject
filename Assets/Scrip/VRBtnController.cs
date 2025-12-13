using UnityEngine;
using UnityEngine.EventSystems;

public class VRButtonController : MonoBehaviour
{
    [SerializeField] private float selectedButtonTime = 2.0f;  // 버튼 응시 시간
    
    private bool isButtonPressed = false;
    private float pressedTime = 0.0f;

    private void Start()
    {
        ResetButton();
    }

    private void Update()
    {
        if (isButtonPressed)
        {
            pressedTime += Time.deltaTime;
            
            if (pressedTime >= selectedButtonTime)
            {
                OnClick();
                ResetButton();
            }
        }
    }

    // UI 클릭 이벤트 발생
    public void OnClick()
    {
        Debug.Log("UI OnClick");
        ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), 
            ExecuteEvents.pointerClickHandler);
    }

    // 버튼 초기화
    private void ResetButton()
    {
        pressedTime = 0.0f;
        isButtonPressed = false;
    }

    // Pointer Enter - 버튼을 바라보기 시작할 때
    public void OnPointerEnter()
    {
        Debug.Log("UI OnPointerEnter");
        isButtonPressed = true;
        ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), 
            ExecuteEvents.pointerEnterHandler);
    }

    // Pointer Exit - 버튼에서 시선을 떼었을 때
    public void OnPointerExit()
    {
        Debug.Log("UI OnPointerExit");
        ResetButton();
        ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), 
            ExecuteEvents.pointerExitHandler);
    }
}