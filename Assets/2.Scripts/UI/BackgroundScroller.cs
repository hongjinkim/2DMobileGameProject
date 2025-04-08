using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{              
    public float backgroundWidth = 37.8f;

    [SerializeField]private GameObject[] backgroundObjects;

    private Camera mainCamera;
    private float screenWidthInWorldUnits;   // 화면 너비
    private float leftScreenEdge;            // 화면 왼쪽 가장자리
    private float rightScreenEdge;           // 화면 오른쪽 가장자리

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // 화면 크기 계산
        CalculateScreenDimensions();
    }

    void Update()
    {
        // 카메라 가장자리 위치 업데이트
        UpdateScreenEdges();

        // 각 배경 오브젝트의 위치 확인 및 업데이트
        UpdateBackgroundPositions();
    }

    void CalculateScreenDimensions()
    {
        Vector3 lowerLeftCorner = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperRightCorner = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        screenWidthInWorldUnits = upperRightCorner.x - lowerLeftCorner.x;
    }

    void UpdateScreenEdges()
    {
        leftScreenEdge = mainCamera.transform.position.x - (screenWidthInWorldUnits / 2);
        rightScreenEdge = mainCamera.transform.position.x + (screenWidthInWorldUnits / 2);
    }

    void UpdateBackgroundPositions()
    {
        // 각 배경 오브젝트 검사
        for (int i = 0; i < backgroundObjects.Length; i++)
        {
            GameObject bg = backgroundObjects[i];
            float bgRightEdge = bg.transform.position.x + backgroundWidth;
            float bgLeftEdge = bg.transform.position.x;

            // 배경이 화면 왼쪽으로 완전히 사라졌을 때
            if (bgRightEdge < leftScreenEdge)
            {
                // 가장 오른쪽에 있는 배경 찾기
                float rightmostX = FindRightmostBackgroundX();

                // 찾은 위치 오른쪽으로 이동
                bg.transform.position = new Vector3(rightmostX + backgroundWidth, bg.transform.position.y, bg.transform.position.z);
            }
            // 배경이 화면 오른쪽으로 완전히 사라졌을 때 (역방향 이동을 지원하기 위한 코드)
            //else if (bgLeftEdge > rightScreenEdge)
            //{
            //    // 가장 왼쪽에 있는 배경 찾기
            //    float leftmostX = FindLeftmostBackgroundX();

            //    // 찾은 위치 왼쪽으로 이동
            //    bg.transform.position = new Vector3(leftmostX - backgroundWidth, bg.transform.position.y, bg.transform.position.z);
            //}
        }
    }

    // 가장 오른쪽에 있는 배경의 X 위치 찾기
    float FindRightmostBackgroundX()
    {
        float rightmostX = float.MinValue;

        foreach (GameObject bg in backgroundObjects)
        {
            if (bg.transform.position.x > rightmostX)
            {
                rightmostX = bg.transform.position.x;
            }
        }

        return rightmostX;
    }

    // 가장 왼쪽에 있는 배경의 X 위치 찾기
    float FindLeftmostBackgroundX()
    {
        float leftmostX = float.MaxValue;

        foreach (GameObject bg in backgroundObjects)
        {
            if (bg.transform.position.x < leftmostX)
            {
                leftmostX = bg.transform.position.x;
            }
        }

        return leftmostX;
    }

    // 화면 크기 변경 시 자동 업데이트
    void OnValidate()
    {
        if (mainCamera != null)
        {
            CalculateScreenDimensions();
        }
    }
}
