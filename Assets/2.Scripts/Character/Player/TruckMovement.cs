using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;
using DG.Tweening;

public class TruckMovement : MonoBehaviour
{
    [SerializeField] private Transform background;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 1.5f;
    [SerializeField] private float deceleration = 2f;
    public Ease accelerationEase = Ease.OutQuad;
    public Ease decelerationEase = Ease.InQuad;

    [Header("Wheel Settings")]
    [SerializeField] private Transform[] wheelObject;
    [SerializeField] private float wheelRotationSpeed = 200f;

    //public LayerMask enemyLayer;

    public float currentSpeed = 0f;
    private bool isMoving = false;
    private bool canStart = true;

    private Tween speedTween;

    private void Start()
    {

    }

    void Update()
    {
        // 시작 시, 적 모두 처치 시 이동 시작
        if (canStart && !isMoving)
        {
            SpeedUpAndGo();
        }
        // 적 감지
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, enemyLayer);

        //if (hit.collider != null && !isDecelerating)
        //{
        //    // 적 감지 시 감속 시작
        //    StartCoroutine(SlowDownAndStop());
        //}

        // 바퀴 회전 (움직일 때만)
        if (isMoving && wheelObject != null)
        {
            foreach(var obj in wheelObject)
                obj.Rotate(0, 0, -wheelRotationSpeed * Time.deltaTime * (currentSpeed / moveSpeed));
        }

        if (isMoving)
        {
            background.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
    }

    public void SlowDownAndStop()
    {
        if (!isMoving) return;

        if (speedTween != null) speedTween.Kill();

        speedTween = DOTween.To(() => currentSpeed, x => currentSpeed = x, 0, deceleration)
            .SetEase(decelerationEase)
            .OnComplete(() => {
                isMoving = false;
                canStart = true;
            });
    }
    public void SpeedUpAndGo()
    {
        if (!canStart || isMoving) return;

        isMoving = true;
        canStart = false;

        if (speedTween != null) speedTween.Kill();

        speedTween = DOTween.To(() => currentSpeed, x => currentSpeed = x, moveSpeed, acceleration)
            .SetEase(accelerationEase);
    }

    private void OnDestroy()
    {
        if (speedTween != null) speedTween.Kill();
    }
}
