using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputHandler : BasicSingleton<InputHandler>
{
    private Camera mainCamera;
    private bool isTrackingTouch = false;
    private int trackedFingerId = -1;

    private Vector3 TouchWorldPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        // Enhanced Touch 지원 활성화
        EnhancedTouchSupport.Enable();
        // 에디터에서 마우스로 터치 시뮬레이션 활성화
        TouchSimulation.Enable();
    }

    private void OnDisable()
    {
        // 에디터 시뮬레이션 비활성화
        TouchSimulation.Disable();
        // Enhanced Touch 지원 비활성화
        EnhancedTouchSupport.Disable();

        isTrackingTouch = false;
        trackedFingerId = -1;
    }

    private void Update()
    {
        // 활성화된 터치가 없으면 종료
        if (Touch.activeTouches.Count == 0)
        {
            if (isTrackingTouch)
                isTrackingTouch = false;
            return;
        }

        // 현재 추적 중인 터치가 없다면, 새로운 터치 시작 감지
        if (!isTrackingTouch)
        {
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    // 첫 번째 터치 시작 감지 시 추적 시작
                    isTrackingTouch = true;
                    trackedFingerId = touch.finger.index;
                    ProcessTouchPosition(touch, "시작");
                    break; // 첫 번째 터치만 처리
                }
            }
        }
        else
        {
            // 이미 추적 중인 터치가 있는 경우, 해당 터치만 계속 추적
            bool foundTrackedTouch = false;

            foreach (var touch in Touch.activeTouches)
            {
                if (touch.finger.index == trackedFingerId)
                {
                    foundTrackedTouch = true;

                    // 추적 중인 터치의 상태에 따라 처리
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                        touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                    {
                        ProcessTouchPosition(touch, "이동 중");
                    }
                    else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                             touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                    {
                        ProcessTouchPosition(touch, "종료");
                        isTrackingTouch = false;
                        trackedFingerId = -1;
                    }
                    break;
                }
            }

            // 추적 중인 터치를 찾지 못한 경우
            if (!foundTrackedTouch)
            {
                Debug.Log("추적 중인 터치가 갑자기 사라졌습니다.");
                isTrackingTouch = false;
                trackedFingerId = -1;
            }
        }
    }
    private void ProcessTouchPosition(Touch touch, string state)
    {
        // 터치 위치 가져오기
        Vector2 touchPosition = touch.screenPosition;

        // 스크린 좌표를 월드 좌표로 변환
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        worldPosition.z = 0; // 2D에서는 z축을 0으로 설정

        // 현재 터치 위치 업데이트
        TouchWorldPosition = worldPosition;

        Debug.Log($"터치 {state} - 위치: {worldPosition}");

        // 터치 위치에서 레이캐스트 수행
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        if (hit.collider != null)
        {
            Debug.Log($"현재 터치 중인 오브젝트: {hit.collider.gameObject.name}");

            // 여기서 오브젝트와의 상호작용 처리
            // 예: 드래깅, 위치 업데이트 등
        }
    }

    // 현재 터치 위치를 외부에서 사용할 수 있는 접근자
    public static Vector3 GetCurrentTouchPosition()
    {
        return Instance.TouchWorldPosition;
    }

    // 현재 터치가 활성화되어 있는지 확인
    public static bool IsTouchActive()
    {
        return Instance.isTrackingTouch;
    }


}